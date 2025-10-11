
using AlexeyKuznetsov.Logger;
using CommandSystem.Console.Core;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading.Channels;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using UnityCommander.SystemMetrics;

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
        private readonly IFileCategorizer _categorizer;
        private readonly ICopyErrorHandler? _errorHandler;
        private readonly ICopySuccessHandler? _successHandler;
        private readonly ICopyMetricsCollector? _metrics;
        private readonly IConsoleOutput _consoleOutput;
        private readonly SerilogCopyLogger _copylogger;
        private const long SmallFileThreshold = 64 * 1024; // 64 KB
        private readonly Subject<ProgressInfo> _progressSubject = new();
        private IEnumerable<DiscoveredItem>? _plannedItems;
        public IObservable<ProgressInfo> ProgressStream => _progressSubject;

        public CopyManager(
            IFileCopier fileCopier,
            IFileCopyPlanner fileCopyPlanner,
            IProgressTracker progressTracker,
            IProgressReporter progressReporter,
            IFileCategorizer smartCategorizer,
            IConsoleOutput? consoleOutput = null,
            ICopyErrorHandler? errorHandler = null,
            ICopySuccessHandler? successHandler = null,
            ICopyMetricsCollector? metrics = null)
        {
            _fileCopier = fileCopier ?? throw new ArgumentNullException(nameof(fileCopier));
            _fileCopyPlanner = fileCopyPlanner ?? throw new ArgumentNullException(nameof(fileCopyPlanner));
            _progressTracker = progressTracker ?? throw new ArgumentNullException(nameof(progressTracker));
            _progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
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

            if (options.UseProgressiveDiscovery)
            {
                await CopyFilesAsyncStreamed(source, target, session, options);
                return;
            }

            // —=== текущая (безопасная) ветка: собираем весь план и копируем ===—
            _plannedItems = await _fileCopyPlanner.GetDiscoveredItems(source, target, options, session.CancellationToken);
            var files = _plannedItems.OnlyFiles().ToList();
            var dirs = _plannedItems.OnlyDirectories().ToList();

            if (!files.Any())
                return;

            long totalBytes = files.Sum(f => new FileInfo(f.Source).Length);
            session.StartSession(totalBytes, files.Count);
            _progressTracker.Start(totalBytes, files.Count);

            if (!options.UseCategories)  // при категоризации папки создавать не нужно
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
            // Временное решение : экспорт метрик по завершении
            //var exp = _metrics?.StopAndCollectReport();
            //session.ExportMetrics(exp);
        }

        private async Task CopyFilesAsyncStreamed(string source, string target, CopySessionService session, CopyOptions options)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(session.CancellationToken);
            var token = cts.Token;

            // bounded channel — чтобы не захламлять память
            var channel = Channel.CreateBounded<DiscoveredItem>(new BoundedChannelOptions(1024)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = true,
                SingleReader = false
            });

            // Producer: планер асинхронно пишет найденные файлы в канал
            var producer = Task.Run(async () =>
            {
                try
                {
                    await foreach (var item in _fileCopyPlanner.GetDiscoveredItemsAsyncEnumerable(source, target, options, token))
                    {
                        token.ThrowIfCancellationRequested();

                        if (item.Type != DiscoveredItemType.File)
                        {
                            // если нужна предварительная обработка директорий в нет-качай режиме — опционально пропускаем
                            continue;
                        }

                        // Обновляем totals на лету
                        session.AddToTotalFiles(1);
                        session.AddToTotalBytes(item.FileSize);
                        _progressTracker.IncrementTotalBytes(item.FileSize); // если у тебя такой метод в трекере; если нет — можно добавить

                        // Пишем в канал
                        await channel.Writer.WriteAsync(item, token);
                    }
                }
                catch (OperationCanceledException) { /* отмена */ }
                finally
                {
                    channel.Writer.TryComplete();
                }
            }, token);

            // Перед началом работы мы можем вызвать StartSession с накопленными 0 (в UI будет 0 пока растёт)
            session.StartSession(0, 0);
            _progressTracker.Start(0, 0);

            // Consumers: запускаем N консьюмеров, в зависимости от options.UseDualChannels/MaxConcurrentTasks
            int consumerCount = options.UseDualChannels ? Math.Max(1, options.MaxConсurrentTasks * 2) : Math.Max(1, options.MaxConсurrentTasks);
            var consumers = Enumerable.Range(0, consumerCount)
                .Select(_ => Task.Run(async () =>
                {
                    await foreach (var item in channel.Reader.ReadAllAsync(token))
                    {
                        // Обработай каждый файл через твой CopySingleFileAsync (он уже корректно обрабатывает категорию vs destination)
                        try
                        {
                            // В streaming режиме destination уже установлен в item.Destination
                            await CopySingleFileAsync(item, target, session, options, token);
                        }
                        catch (OperationCanceledException)
                        {
                            if (_plannedItems != null)
                                session.CleanupAfterCancel(_plannedItems);
                            session.UpdateFileStatus(item.Source, FileCopyStatus.Failed);
                            // при отмене — просто прерываем, остальные потребители увидят Completion
                        }
                        catch (Exception ex)
                        {
                            session.UpdateFileStatus(item.Source, FileCopyStatus.Failed);
                            _metrics?.OnError(item.Source, ex);
                            _copylogger.LogCopyError(item.Source, item.Destination, ex);
                        }
                    }
                }, token)).ToArray();

            // Ждём всех
            await Task.WhenAll(consumers.Prepend(producer));
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
            // Если отключена многопоточность, используем только один поток
            int concurrency = options.UseMultiThreading ? maxConcurrentTasks : 1;
            var target = session.CurrentSession.TargetPath;
            using var semaphore = new SemaphoreSlim(concurrency);
            _metrics?.PrepareAllFilesCopy(session.CurrentSession.SourcePath, session.CurrentSession.TargetPath, options.UseMetrics);
            var tasks = files.Select(file => Task.Run(async () =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    
                    try
                    {
                        await CopySingleFileAsync(file, target, session, options, cancellationToken);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
                catch (OperationCanceledException)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
                    if (_plannedItems != null)
                        session.CleanupAfterCancel(_plannedItems);
                    // НЕ делаем throw
                }
                catch (Exception ex)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
                    _metrics?.OnError(file.Source, ex);
                    _copylogger.LogCopyError(file.Source, Path.Combine(session.CurrentSession.TargetPath, Path.GetFileName(file.Source)), ex);
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
            string destinationFile = file.Destination;
            session.OnFileStarted(file.Source, destinationFile, file.FileSize);
            Debug.WriteLine("[CopySingleFileAsync] StartFile");
            _progressTracker.StartFile(file.Source, new FileInfo(file.Source).Length);
            // Запускаем секундомер для измерения времени копирования одного файла
            var stopwatch = Stopwatch.StartNew();
            int bufferSize = GetBufferSize(file.Source, options);

            await _fileCopier.CopyFileAsync(
                file.Source,
                destinationFile,
                bufferSize,
                bytesCopied =>
                {
                    session.Controller.WaitIfPaused();
                    cancellationToken.ThrowIfCancellationRequested();
                    _progressTracker.UpdateProgress(bytesCopied);
                    session.UpdateFileProgress(file.Source, bytesCopied);
                    var info = _progressTracker.GetProgressInfo();
                    _progressReporter.Report(info);
                    //Debug.WriteLine($"CurrentFilePath={_progressTracker.GetProgressInfo().CurrentFilePath}, Bytes={_progressTracker.GetProgressInfo().CurrentFileCopiedBytes}");
                },
                cancellationToken,
                session.Controller.WaitIfPaused);
            // Останавливаем секундомер — завершение измерения времени
            stopwatch.Stop();
            // Уведомляем систему метрик о завершении копирования: путь, размер, затраченное время
            session.UpdateFileStatus(file.Source, FileCopyStatus.Completed);
            _metrics?.OnFileCopyCompleted(file.Source, destinationFile, file.FileSize, stopwatch.Elapsed);
            _progressTracker.CompleteFile();
            Debug.WriteLine("[CopySingleFileAsync] CompleteFile called");
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
