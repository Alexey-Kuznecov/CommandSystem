
using AlexeyKuznetsov.Logger;
using CommandSystem.Console.Core;
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
        private readonly ICopyErrorHandler? _errorHandler;
        private readonly ICopySuccessHandler? _successHandler;
        private readonly ICopyMetricsCollector _metrics;
        private IConsoleOutput _consoleOutput;
        private SerilogCopyLogger _copylogger;

        public CopyManager(
            IFileCopier fileCopier,
            IFileCopyPlanner fileCopyPlanner,
            IProgressTracker progressTracker,
            IProgressReporter progressReporter,
            ICopyErrorHandler? errorHandler = null,
            ICopySuccessHandler? successHandler = null,
            ICopyMetricsCollector? metrics = null)
        {
            _fileCopier = fileCopier;
            _fileCopyPlanner = fileCopyPlanner;
            _progressTracker = progressTracker;
            _progressReporter = progressReporter;
            _errorHandler = errorHandler;
            _successHandler = successHandler; // Removed redundant null assignment
            _copylogger = new SerilogCopyLogger();
            _metrics = metrics ?? new NullCopyMetricsCollector(); // <= безопасно
        }

        public async Task CopyFilesAsync(string sourceDirectory, string destinationDirectory, CopyOptions options, CancellationToken cancellationToken)
        {
            //_progressTracker.Start(0, 0);
            var plannedItems = await _fileCopyPlanner.GetDiscoveredItems(sourceDirectory, destinationDirectory, options, cancellationToken);
            //var folderTracker = new FolderSizeTracker(sourceDirectory);
            if (!plannedItems.Any())
                return;

            var files = plannedItems.OnlyFiles();
            var dirs = plannedItems.OnlyDirectories();

            // ← ТОЛЬКО ОДИН раз вызываем Start с корректными данными
            _progressTracker.Start(files.Sum(f => new FileInfo(f.Source).Length), files.Count());
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir.Source) && !Directory.Exists(dir.Destination))
                {
                    Directory.CreateDirectory(dir.Destination);
                    _metrics.OnDirectoryCreated(dir.Destination);
                }
            }

            int fileIndex = 0; // Logger
            using var semaphore = new SemaphoreSlim(options.MaxConсurrentTasks);
            // Уведомляем систему метрик о начале копирования — может использоваться для сбора статистики (например, активные файлы)
            _metrics.PrepareAllFilesCopy(sourceDirectory, destinationDirectory, options.UseMetrics);

            var tasks = files
                .Select(async file =>
                {
                    //_consoleOutput.WriteLine($"[START_WAIT] {Path.GetFileName(file.Source)} ждёт слот...");
                    await semaphore.WaitAsync(cancellationToken); // <== захватываем слот
                    //_consoleOutput.WriteLine($"[ACQUIRED] {Path.GetFileName(file.Source)} начал копирование");
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested(); // или IsCancellationRequested

                        var source = file.Source;
                        var destination = file.Destination;

                        // Увеличиваем индекс текущего файла для логгера (используется для отслеживания порядка)
                        fileIndex++;

                        // Логируем начало копирования файла, полезно для отладки и аудита
                        _copylogger.LogCopyStarted(source, destination);

                        // Запускаем секундомер для измерения времени копирования одного файла
                        var stopwatch = Stopwatch.StartNew();
                        _progressTracker.StartFile(source, new FileInfo(source).Length); // <== Начинаем отслеживание прогресса
                        // Выполняем асинхронное копирование файла с передачей обработчика прогресса по байтам
                        await _fileCopier.CopyFileAsync(
                            source,
                            destination,
                            bytesCopied =>
                            {
                                _progressTracker.UpdateProgress(bytesCopied);
                                // Отправляем обновлённую информацию о прогрессе для отображения (например, прогресс-бар)
                                _progressReporter.Report(_progressTracker.GetProgressInfo()); // 👈 Репортирует каждые 64 KB
                            },
                            cancellationToken);

                        // Останавливаем секундомер — завершение измерения времени
                        stopwatch.Stop();
                        
                        // Уведомляем трекер о завершении копирования одного файла (кол-во байт уже обновлено во время копирования)
                        _progressTracker.CompleteFile();
                        
                        // Репортирует об окончании копирования файла
                        _progressReporter.Report(_progressTracker.GetProgressInfo());

                        // Получаем информацию о прогрессе
                        var progressInfo = _progressTracker.GetProgressInfo();
                        
                        // Уведомляем систему метрик о завершении копирования: путь, размер, затраченное время
                        _metrics.OnFileCopyCompleted(source, destination, file.FileSize, stopwatch.Elapsed);

                        // Логируем завершение копирования: пути, размер, время и индекс — используется для анализа и отладки
                        _copylogger.LogCopyCompleted(source, destination, file.FileSize, stopwatch.Elapsed, fileIndex);

                        //_successHandler.HandleSuccess(new FileCopySuccessContext(source, destination, new FileInfo(file.Source).Length));
                    }
                    catch (OperationCanceledException)
                    {
                        // Можно логировать отмену, если надо:
                        _consoleOutput.WriteLine("\n[CopyManager] Операция копирования была отменена.");
                        throw; // Выход из цикла, если отмена
                    }
                    catch (Exception ex)
                    {
                        _metrics.OnError(file.Source, ex);
                        _copylogger.LogCopyError(file.Source, file.Destination, ex); // Logger
                        //var context = new FileCopyErrorContext(file.Source, file.Destination, ex);
                        //_errorHandler.HandleError(context);
                    }
                    finally
                    {
                        semaphore.Release(); // <== освобождаем слот
                        //_consoleOutput.WriteLine($"[RELEASED] {Path.GetFileName(file.Source)} освободил слот");
                    }
                });

            if (options.UseMultiThreading)
            {
                await Task.WhenAll(tasks);
                _metrics.ReportFinal();
            }
            else
            {
                foreach (var task in tasks) 
                { 
                    try
                    {
                        await task;
                    }
                    catch (OperationCanceledException)
                    {
                        // Можно логировать отмену, если надо:
                        //_consoleOutput.WriteLine("[CopyManager] Операция копирования была отменена.");
                        break; // Выход из цикла, если отмена
                    }
                }
                _metrics.ReportFinal();
            }
        }
    }
}
