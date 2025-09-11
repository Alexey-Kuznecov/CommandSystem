
using AlexeyKuznetsov.Logger;
using CommandSystem.Console.Core;
using CommandSystem.CopyTester.ViewModels;
using System.Diagnostics;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;
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
        private readonly ICopyMetricsCollector? _metrics;
        private readonly IConsoleOutput _consoleOutput; // Marked as readonly and initialized in constructor
        private readonly SerilogCopyLogger _copylogger;

        public CopyManager(
            IFileCopier fileCopier,
            IFileCopyPlanner fileCopyPlanner,
            IProgressTracker progressTracker,
            IProgressReporter progressReporter,
            IConsoleOutput? consoleOutput = null, // Added as a required parameter
            ICopyErrorHandler? errorHandler = null,
            ICopySuccessHandler? successHandler = null,
            ICopyMetricsCollector? metrics = null)
        {
            _fileCopier = fileCopier;
            _fileCopyPlanner = fileCopyPlanner;
            _progressTracker = progressTracker;
            _progressReporter = progressReporter;
            _consoleOutput = consoleOutput; // Initialize _consoleOutput
            _errorHandler = errorHandler;
            _successHandler = successHandler;
            _copylogger = new SerilogCopyLogger();
            _metrics = metrics ?? new NullCopyMetricsCollector(); // <= безопасно
        }

        // --- НОВАЯ перегрузка: основная реализация, работающая через CopySessionService ---
        public async Task CopyFilesAsync(CopySessionService session, CancellationToken cancellationToken)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrWhiteSpace(session.SourcePath) || string.IsNullOrWhiteSpace(session.TargetPath))
                return;

            // Получаем план (файлы и директории)
            var plannedItems = await _fileCopyPlanner.GetDiscoveredItems(session.SourcePath, session.TargetPath, session.Options, cancellationToken);
            if (!plannedItems.Any())
                return;

            var files = plannedItems.OnlyFiles().ToList();
            var dirs = plannedItems.OnlyDirectories().ToList();

            // Подготовка общей информации
            long totalBytes = files.Sum(f => new FileInfo(f.Source).Length);
            int totalFiles = files.Count;

            // Инициализируем сессию (обнулим счётчики) — если в сессии есть StartSession
            try
            {
                session.StartSession(totalBytes, totalFiles);
            }
            catch
            {
                // Если StartSession отсутствует или ведёт себя иначе — безопасно проглатываем исключение.
                // (Это на случай, если у тебя другой вариант реализации сессии).
            }

            // Инициализируем трекер прогресса один раз
            _progressTracker.Start(totalBytes, totalFiles);

            // Создание директорий
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir.Source) && !Directory.Exists(dir.Destination))
                {
                    Directory.CreateDirectory(dir.Destination);
                    _metrics.OnDirectoryCreated(dir.Destination);
                }
            }

            int fileIndex = 0; // будем увеличивать атомарно
            using var semaphore = new SemaphoreSlim(session.Options?.MaxConсurrentTasks ?? 1);

            _metrics.PrepareAllFilesCopy(session.SourcePath, session.TargetPath, session.Options?.UseMetrics ?? false);

            // Функция для безопасного запуска копирования одного файла
            var tasks = files.Select(file => Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var source = file.Source;
                    var destination = file.Destination;

                    // Атомарный инкремент индекса файла
                    var thisFileIndex = Interlocked.Increment(ref fileIndex);

                    _copylogger.LogCopyStarted(source, destination);

                    var stopwatch = Stopwatch.StartNew();

                    // Начинаем отслеживание файла
                    _progressTracker.StartFile(source, new FileInfo(source).Length);

                    // Копирование с обработчиком прогресса — обёртка, чтобы учитывать паузу и обновлять сессию
                    await _fileCopier.CopyFileAsync(
                        source,
                        destination,
                        bytesCopied =>
                        {
                            session.WaitIfPaused();          // блокируем если на паузе
                            session.Token.ThrowIfCancellationRequested();  // проверка отмены
                            // Обновляем локальный трекер и репортим прогресс
                            _progressTracker.UpdateProgress(bytesCopied);
                            // Обновляем fields сессии
                            try { session.AddBytes(bytesCopied); } catch { /* безопасно */ }

                            // Отправляем репорт всем подписчикам
                            _progressReporter.Report(_progressTracker.GetProgressInfo());
                        },
                        cancellationToken, session.WaitIfPaused).ConfigureAwait(false);

                    session.AddCopiedFile(destination); // <-- добавляем только после успешного завершения файла
                    stopwatch.Stop();

                    // После завершения файла
                    _progressTracker.CompleteFile();
                    try { session.CompleteFile(); } catch { /* безопасно */ }

                    // Финальный репорт после файла
                    _progressReporter.Report(_progressTracker.GetProgressInfo());

                    // Обновляем метрики/логи
                    _metrics.OnFileCopyCompleted(source, destination, file.FileSize, stopwatch.Elapsed);
                    _copylogger.LogCopyCompleted(source, destination, file.FileSize, stopwatch.Elapsed, thisFileIndex);

                    // При желании: _successHandler.HandleSuccess(...)
                }
                catch (OperationCanceledException)
                {
                    //_consoleOutput.WriteLine("\n[CopyManager] Операция копирования была отменена.");
                    session.CleanupAfterCancel(plannedItems);
                    throw;
                }
                catch (Exception ex)
                {
                    _metrics.OnError(file.Source, ex);
                    _copylogger.LogCopyError(file.Source, file.Destination, ex);
                    // Можно вызвать _errorHandler.HandleError(...)
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken)).ToArray();

            // Выполнение задач (многопоточно или последовательно)
            if (session.Options?.UseMultiThreading ?? true)
            {
                try
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // отмена — безопасно прерываем
                }
            }
            else
            {
                foreach (var t in tasks)
                {
                    try
                    {
                        await t.ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }

            // Финальный отчёт метрик
            _metrics.ReportFinal();
        }
    }
}
