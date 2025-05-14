
using AlexeyKuznetsov.Logger;
using System.Diagnostics;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying
{
    public class CopyManager //: ICopyManager
    {
        private readonly IFileCopier _fileCopier;
        private readonly IFileCopyPlanner _fileCopyPlanner;
        private readonly IProgressTracker _progressTracker;
        private readonly IProgressReporter _progressReporter;
        private readonly ICopyErrorHandler _errorHandler;
        private readonly ICopySuccessHandler _successHandler;
        private readonly ICopyMetricsCollector _metrics;
        private SerilogCopyLogger _logger;

        public CopyManager(
            IFileCopier fileCopier,
            IFileCopyPlanner fileCopyPlanner,
            IProgressTracker progressTracker,
            IProgressReporter progressReporter,
            ICopyErrorHandler errorHandler,
            ICopySuccessHandler successHandler,
            ICopyMetricsCollector? metrics = null)
        {
            _fileCopier = fileCopier;
            _fileCopyPlanner = fileCopyPlanner;
            _progressTracker = progressTracker;
            _progressReporter = progressReporter;
            _errorHandler = errorHandler;
            _successHandler = successHandler;
            _logger = new SerilogCopyLogger();
            _metrics = metrics ?? new NullCopyMetricsCollector(); // <= безопасно
        }

        public async Task CopyFilesAsync(string sourceDirectory, string destinationDirectory, CopyOptions options, CancellationToken cancellationToken)
        {
            //_progressTracker.Start(0, 0);
            var plannedItems = await _fileCopyPlanner.GetDiscoveredItems(sourceDirectory, destinationDirectory, options, cancellationToken);
            var folderTracker = new FolderSizeTracker(sourceDirectory);
            if (!plannedItems.Any())
                return;

            var files = plannedItems.OnlyFiles();
            var dirs = plannedItems.OnlyDirectories();

            // ← ТОЛЬКО ОДИН раз вызываем Start с корректными данными
            _progressTracker.Start(files.Sum(f => new FileInfo(f.Source).Length), files.Count());
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir.Source) && !Directory.Exists(dir.Destination))
                    Directory.CreateDirectory(dir.Destination);
            }

            int fileIndex = 0; // Logger

            var tasks = files
                .Select(async file =>
                {
                    try
                    {
                        var source = file.Source;

                        fileIndex++; // Logger
                        var sw = Stopwatch.StartNew(); // Logger
                        _logger.LogCopyStarted(source, file.Destination); // Logger
                        await _fileCopier.CopyFileAsync(source, file.Destination,
                        bytesCopied => _progressTracker.UpdateProgress(bytesCopied), cancellationToken);
                        sw.Stop(); // Logger

                        var fileInfo = new FileInfo(source);
                        _metrics.OnFileCopyCompleted(source, file.Destination, fileInfo.Length, sw.Elapsed);

                        _logger.LogCopyCompleted(source, file.Destination, file.FileSize, sw.Elapsed, fileIndex); // Logger
                        _progressTracker.CompleteFile(); // <-- только файл, байты уже учтены
                        _progressReporter.Report(_progressTracker.GetProgressInfo());

                        _successHandler.HandleSuccess(new FileCopySuccessContext(source, file.Destination, new FileInfo(file.Source).Length));
                    }
                    catch (Exception ex)
                    {

                        _logger.LogCopyError(file.Source, file.Destination, ex); // Logger
                        var context = new FileCopyErrorContext(file.Source, file.Destination, ex);
                        _errorHandler.HandleError(context);
                    }
                });

            if (options.UseMultiThreading)
            {
                await Task.WhenAll(tasks);
            }
            else
            {
                foreach (var task in tasks)
                {
                    await task;
                }
            }
        }
    }
}
