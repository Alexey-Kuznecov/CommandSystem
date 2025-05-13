
using System.Diagnostics;
using System.IO;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
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

        public CopyManager(
            IFileCopier fileCopier,
            IFileCopyPlanner fileCopyPlanner,
            IProgressTracker progressTracker,
            IProgressReporter progressReporter,
            ICopyErrorHandler errorHandler,
            ICopySuccessHandler successHandler)
        {
            _fileCopier = fileCopier;
            _fileCopyPlanner = fileCopyPlanner;
            _progressTracker = progressTracker;
            _progressReporter = progressReporter;
            _errorHandler = errorHandler;
            _successHandler = successHandler;
        }

        public async Task CopyFilesAsync(string sourceDirectory, string destinationDirectory, CopyOptions options, CancellationToken cancellationToken)
        {
            // Стартуем трекинг прогресса
            _progressTracker.Start(0, 0); // Изначально прогресс 0

            var plannedFiles = await _fileCopyPlanner.GetFilesToCopyAsync(sourceDirectory, destinationDirectory, options, cancellationToken);

            // Если нет файлов, ничего не копируем
            if (!plannedFiles.Any())
            {
                return;
            }

            // Обновляем прогресс
            _progressTracker.Start(plannedFiles.Sum(p => new FileInfo(p.Source).Length), plannedFiles.Count());

            var tasks = plannedFiles
                .Select(async file =>
                {
                    try
                    {
                        var source = file.Source;
                        file.Destination = file.Source.Replace(sourceDirectory, destinationDirectory);

                        if (Directory.Exists(file.Source))
                        {
                            // Это папка — создаём такую же в целевой директории
                            if (!Directory.Exists(file.Destination))
                                Directory.CreateDirectory(file.Destination);
                        }
                        else if (File.Exists(file.Source))
                        {
                            // Это файл — создаём директорию, в которую он будет скопирован
                            if (!File.Exists(file.Destination))
                                Directory.CreateDirectory(Path.GetDirectoryName(file.Destination));
                        }

                        var stopwatch = Stopwatch.StartNew();
                        await _fileCopier.CopyFileAsync(file.Source, file.Destination, cancellationToken);
                        stopwatch.Stop();

                        // Обновляем прогресс
                        _progressTracker.ReportFileCopied(new FileInfo(file.Source).Length, 1);
                        _progressReporter.Report(_progressTracker.GetProgressInfo());

                        // Логируем успех
                        _successHandler.HandleSuccess(new FileCopySuccessContext(file.Source, file.Destination, new FileInfo(file.Source).Length, stopwatch.Elapsed));
                    }
                    catch (Exception ex)
                    {
                        var context = new FileCopyErrorContext(file.Source, file.Destination, ex);
                        _errorHandler.HandleError(context);
                    }
                });

            // В зависимости от настроек, копируем файлы с многозадачностью или последовательно
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
