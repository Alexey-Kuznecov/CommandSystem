
using AlexeyKuznetsov.Logger;
using CommandSystem.Console.Core;
using System.Diagnostics;
using System.Reactive.Subjects;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying
{
    /// <summary>
    /// Класс <c>CopyManager</c> отвечает за выполнение процесса копирования файлов и директорий
    /// с учётом различных стратегий планирования, отслеживания прогресса, обработки ошибок и метрик.
    /// Он разделяет работу с большими и маленькими файлами для оптимизации производительности
    /// и поддерживает асинхронное многопоточное копирование.
    /// </summary>
    public class CopyManager : IDisposable
    {
        private readonly IFileCopier _fileCopier;
        private readonly IFileCopyPlanner _fileCopyPlanner;
        private readonly IProgressTracker _progressTracker;
        private readonly IProgressReporter _progressReporter;
        private readonly ICopyFileReporter _copyFileReporter;
        private readonly ISmartCategorizer _categorizer;
        private readonly ICopyErrorHandler? _errorHandler;
        private readonly ICopySuccessHandler? _successHandler;
        private readonly ICopyMetricsCollector? _metrics;
        private readonly IConsoleOutput _consoleOutput;
        private readonly SerilogCopyLogger _copylogger;
        private const long SmallFileThreshold = 64 * 1024; // 64 KB
        private readonly Subject<ProgressInfo> _progressSubject = new();
        public IObservable<ProgressInfo> ProgressStream => _progressSubject;

        public CopyManager(
            IFileCopier fileCopier,
            IFileCopyPlanner fileCopyPlanner,
            IProgressTracker progressTracker,
            IProgressReporter progressReporter,
            ICopyFileReporter copyFileReporter,
            ISmartCategorizer smartCategorizer,
            IConsoleOutput? consoleOutput = null,
            ICopyErrorHandler? errorHandler = null,
            ICopySuccessHandler? successHandler = null,
            ICopyMetricsCollector? metrics = null)
        {
            _fileCopier = fileCopier ?? throw new ArgumentNullException(nameof(fileCopier));
            _fileCopyPlanner = fileCopyPlanner ?? throw new ArgumentNullException(nameof(fileCopyPlanner));
            _progressTracker = progressTracker ?? throw new ArgumentNullException(nameof(progressTracker));
            _progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
            _copyFileReporter = copyFileReporter;
            _categorizer = smartCategorizer;
            _consoleOutput = consoleOutput ?? new NullConsoleOutput();
            _errorHandler = errorHandler;
            _successHandler = successHandler;
            _metrics = metrics ?? new NullCopyMetricsCollector();
            _copylogger = new SerilogCopyLogger();

            _progressReporter.ProgressChanged += info => _progressSubject.OnNext(info);
        }

        #region Public Copy Methods

        public async Task CopyFilesAsync(string source, string target, CopySessionService session, CompositeCopySettings settings)
        {
            var options = new CopyOptions();
            settings.Apply(ref options);

            var plannedItems = await _fileCopyPlanner.GetDiscoveredItems(source, target, options, session.CancellationToken);

            var files = plannedItems.OnlyFiles().ToList();
            var dirs = plannedItems.OnlyDirectories().ToList();

            if (!files.Any())
                return;

            long totalBytes = files.Sum(f => new FileInfo(f.Source).Length);
            session.StartSession(totalBytes, files.Count);
            _progressTracker.Start(totalBytes, files.Count);

            CreateDirectories(dirs);

            if (options.UseDualChannels)
            {
                var (smallFiles, largeFiles) = SplitFilesBySize(files);

                await Task.WhenAll(
                    ProcessFilesAsync(largeFiles, session, options, options.MaxConсurrentTasks, session.CancellationToken),
                    ProcessFilesAsync(smallFiles, session, options, options.MaxConсurrentTasks * 2, session.CancellationToken)
                );
            }
            else
            {
                await ProcessFilesAsync(files, session, options, options.MaxConсurrentTasks, session.CancellationToken);
            }

            _metrics?.ReportFinal();
        }

        /// <summary>
        /// Старый метод для тестирования/референса — одноканальный, без разделения на большие/малые файлы.
        /// </summary>
        public async Task CopyFilesAsyncOld(CopySessionService session, CopyOptions options, CancellationToken cancellationToken)
        {
            var plannedItems = await _fileCopyPlanner.GetDiscoveredItems(session.SourcePath, session.TargetPath, options, cancellationToken);

            var files = plannedItems.OnlyFiles().ToList();
            var dirs = plannedItems.OnlyDirectories().ToList();

            if (!files.Any())
                return;

            long totalBytes = files.Sum(f => new FileInfo(f.Source).Length);
            session.StartSession(totalBytes, files.Count);
            _progressTracker.Start(totalBytes, files.Count);

            CreateDirectories(dirs);

            await ProcessFilesAsync(files, session, options, options.MaxConсurrentTasks, cancellationToken);

            _metrics?.ReportFinal();
        }

        #endregion

        #region Internal Helpers

        private void CreateDirectories(IEnumerable<DiscoveredItem> dirs)
        {
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir.Source) && !Directory.Exists(dir.Destination))
                {
                    Directory.CreateDirectory(dir.Destination);
                    _metrics?.OnDirectoryCreated(dir.Destination);
                }
            }
        }

        private (List<DiscoveredItem> smallFiles, List<DiscoveredItem> largeFiles) SplitFilesBySize(List<DiscoveredItem> files)
        {
            var smallFiles = new List<DiscoveredItem>();
            var largeFiles = new List<DiscoveredItem>();

            foreach (var file in files)
            {
                var size = new FileInfo(file.Source).Length;
                if (size < SmallFileThreshold)
                    smallFiles.Add(file);
                else
                    largeFiles.Add(file);
            }

            return (smallFiles, largeFiles);
        }

        private async Task ProcessFilesAsync(
            IEnumerable<DiscoveredItem> files,
            CopySessionService session,
            CopyOptions options,
            int maxConcurrentTasks,
            CancellationToken cancellationToken)
        {
            using var semaphore = new SemaphoreSlim(maxConcurrentTasks);

            var tasks = files.Select(file => Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await CopySingleFileAsync(file, session.TargetPath, session, options, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
                    throw;
                }
                catch (Exception ex)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
                    _metrics?.OnError(file.Source, ex);
                    _copylogger.LogCopyError(file.Source, Path.Combine(session.TargetPath, Path.GetFileName(file.Source)), ex);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken)).ToArray();

            await Task.WhenAll(tasks);
        }

        private async Task CopySingleFileAsync(
            DiscoveredItem file,
            string destinationRoot,
            CopySessionService session,
            CopyOptions options,
            CancellationToken cancellationToken)
        {
            string destinationFile;

            if (options.UseCategories)
            {
                string category = await _categorizer.CategorizeAsync(new FileInfo(file.Source));
                string categoryDir = Path.Combine(destinationRoot, category);

                if (!Directory.Exists(categoryDir))
                    Directory.CreateDirectory(categoryDir);

                destinationFile = Path.Combine(categoryDir, Path.GetFileName(file.Source));
                session.OnFileStarted(file.Source, destinationFile, file.FileSize, category);
            }
            else
            {
                destinationFile = Path.Combine(destinationRoot, Path.GetFileName(file.Source));
                session.OnFileStarted(file.Source, destinationFile, file.FileSize, "Default");
            }

            _progressTracker.StartFile(file.Source, new FileInfo(file.Source).Length);

            int bufferSize = GetBufferSize(file.Source, options);

            await _fileCopier.CopyFileAsync(
                file.Source,
                destinationFile,
                bufferSize,
                bytesCopied =>
                {
                    session.WaitIfPaused();
                    cancellationToken.ThrowIfCancellationRequested();
                    _progressTracker.UpdateProgress(bytesCopied);
                    session.UpdateFileProgress(file.Source, bytesCopied);
                    _progressReporter.Report(_progressTracker.GetProgressInfo());
                },
                cancellationToken,
                session.WaitIfPaused);

            _progressTracker.CompleteFile();
            session.UpdateFileStatus(file.Source, FileCopyStatus.Completed);
        }

        private int GetBufferSize(string sourcePath, CopyOptions options)
        {
            long fileLength = new FileInfo(sourcePath).Length;

            if (fileLength < options.BufferSize)
                return Math.Max((int)fileLength, options.MinBufferSize);

            return options.BufferSize;
        }

        #endregion

        public void Dispose()
        {
            _progressSubject.OnCompleted();
            _progressSubject.Dispose();
        }
    }
}
