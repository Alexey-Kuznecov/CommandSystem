
using AlexeyKuznetsov.Logger;
using CommandSystem.Console.Core;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
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
    public class CopyManager //: ICopyManager
    {
        #region Поля

        /// <summary>
        /// Стратегия копирования файлов. Отвечает за низкоуровневое копирование с буферизацией.
        /// </summary>
        private readonly IFileCopier _fileCopier;

        /// <summary>
        /// Планировщик файлов для копирования. Определяет, какие файлы и директории нужно копировать
        /// в текущей сессии.
        /// </summary>
        private readonly IFileCopyPlanner _fileCopyPlanner;

        /// <summary>
        /// Трекер прогресса копирования. Поддерживает внутренние счётчики байтов и файлов.
        /// </summary>
        private readonly IProgressTracker _progressTracker;

        /// <summary>
        /// Репортер прогресса копирования. Отвечает за внешнюю визуализацию прогресса (например, консоль, UI).
        /// </summary>
        private readonly IProgressReporter _progressReporter;
        
        private readonly ICopyFileReporter _copyFileReporter;
        private readonly ISmartCategorizer _categorizer;
        /// <summary>
        /// Опциональный обработчик ошибок копирования.
        /// </summary>
        private readonly ICopyErrorHandler? _errorHandler;

        /// <summary>
        /// Опциональный обработчик успешного завершения копирования.
        /// </summary>
        private readonly ICopySuccessHandler? _successHandler;

        /// <summary>
        /// Коллектор метрик копирования (например, скорость, статистика по файлам).
        /// </summary>
        private readonly ICopyMetricsCollector? _metrics;

        /// <summary>
        /// Опциональный вывод в консоль (только для текстовой информации).
        /// </summary>
        private readonly IConsoleOutput _consoleOutput;

        /// <summary>
        /// Логгер для записи операций копирования.
        /// </summary>
        private readonly SerilogCopyLogger _copylogger;

        /// <summary>
        /// Пороговый размер файла, ниже которого файл считается "маленьким" и обрабатывается отдельно.
        /// </summary>
        private const long SmallFileThreshold = 64 * 1024; // 64 KB

        // Subject для проброса прогресса
        private readonly Subject<ProgressInfo> _progressSubject = new();
        private IEnumerable<DiscoveredItem>? _plannedItems;
        public IObservable<ProgressInfo> ProgressStream => _progressSubject;

        #endregion

        #region Конструктор

        /// <summary>
        /// Инициализирует новый экземпляр <c>CopyManager</c> с заданными зависимостями.
        /// </summary>
        /// <param name="fileCopier">Стратегия копирования файлов.</param>
        /// <param name="fileCopyPlanner">Планировщик файлов и директорий.</param>
        /// <param name="progressTracker">Трекер прогресса копирования.</param>
        /// <param name="progressReporter">Репортер прогресса для отображения пользователю.</param>
        /// <param name="consoleOutput">Опциональный вывод сообщений в консоль.</param>
        /// <param name="errorHandler">Опциональный обработчик ошибок копирования.</param>
        /// <param name="successHandler">Опциональный обработчик успешного завершения.</param>
        /// <param name="metrics">Опциональный коллектор метрик; если не передан, используется <c>NullCopyMetricsCollector</c>.</param>
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
            _categorizer = smartCategorizer;
            _copyFileReporter = copyFileReporter;
            _fileCopier = fileCopier ?? throw new ArgumentNullException(nameof(fileCopier));
            _fileCopyPlanner = fileCopyPlanner ?? throw new ArgumentNullException(nameof(fileCopyPlanner));
            _progressTracker = progressTracker ?? throw new ArgumentNullException(nameof(progressTracker));
            _progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
            _consoleOutput = consoleOutput ?? new NullConsoleOutput();
            _errorHandler = errorHandler;
            _successHandler = successHandler;
            _metrics = metrics ?? new NullCopyMetricsCollector();
            _copylogger = new SerilogCopyLogger();

            // Пробросим прогресс в реактивный стрим
            _progressReporter.ProgressChanged += info => _progressSubject.OnNext(info);
        }

        #endregion

        #region Публичные методы

        public async Task CopyFilesAsync(string source, string target, CopySessionService session, CompositeCopySettings settings)
        {
            var options = new CopyOptions();
            settings.Apply(ref options);

            // Определяем количество каналов
            //int largeFileTasks = options.MaxConсurrentTasks;              // большие файлы
            //int smallFileTasks = options.UseDualChannels
            //    ? options.MaxConсurrentTasks * 2
            //    : options.MaxConсurrentTasks;                            // маленькие файлы, в одноканальном режиме такое же количество

            //await CopyFilesInternalAsync(session, options, largeFileTasks, smallFileTasks, session.CancellationToken);
            CopyFilesAsyncOld(session,  options, session.CancellationToken);
        }

        /// <summary>
        /// Асинхронно выполняет копирование файлов и директорий в рамках указанной сессии.
        /// </summary>
        /// <param name="session">Сессия копирования, содержащая пути и настройки.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        private async Task CopyFilesInternalAsync(
            CopySessionService session,
            CopyOptions options,
            int largeFileTasks,
            int smallFileTasks,
            CancellationToken cancellationToken)
        {
            // Получаем все файлы и директории
            var (files, dirs) = await PreparePlannedItemsAsync(session.SourcePath, session.TargetPath, options, cancellationToken);
            if (!files.Any())
                return;

            // Суммарный размер файлов
            long totalBytes = files.Sum(f => new FileInfo(f.Source).Length);

            // Инициализация сессии и трекеров прогресса
            session.StartSession(totalBytes, files.Count);
            _progressTracker.Start(totalBytes, files.Count);

            // Создание директорий
            CreateDirectories(dirs);

            // Разделение на маленькие и большие файлы
            var (smallFiles, largeFiles) = SplitFilesBySize(files);

            // Запуск обработки очередей файлов
            var tasks = new List<Task>
            {
                ProcessFileQueueAsync(largeFiles, largeFileTasks, session, options, cancellationToken),
                ProcessFileQueueAsync(smallFiles, smallFileTasks, session, options, cancellationToken)
            };

            await Task.WhenAll(tasks);

            _metrics?.ReportFinal();
        }

        #endregion

        #region Приватные вспомогательные методы

        /// <summary>
        /// Создаёт директории назначения, если они ещё не существуют.
        /// </summary>
        /// <param name="dirs">Список директорий для создания.</param>
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

        /// <summary>
        /// Получает запланированные элементы (файлы и директории) для копирования.
        /// </summary>
        /// <param name="session">Сессия копирования.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Кортеж с двумя списками: файлы и директории.</returns>
        private async Task<(List<DiscoveredItem> files, List<DiscoveredItem> dirs)> PreparePlannedItemsAsync(
            string source, string target, CopyOptions options, CancellationToken cancellationToken)
        {
            _plannedItems = await _fileCopyPlanner
                .GetDiscoveredItems(source, target, options, cancellationToken);

            if (!_plannedItems.Any())
                return (new List<DiscoveredItem>(), new List<DiscoveredItem>());

            var files = _plannedItems.OnlyFiles().ToList();
            var dirs = _plannedItems.OnlyDirectories().ToList();

            return (files, dirs);
        }

        /// <summary>
        /// Разделяет список файлов на маленькие и большие в зависимости от порогового размера.
        /// </summary>
        /// <param name="files">Список файлов для разделения.</param>
        /// <returns>Кортеж с двумя списками: маленькие файлы и большие файлы.</returns>
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

        /// <summary>
        /// Асинхронно обрабатывает очередь файлов с ограничением по количеству одновременных задач.
        /// </summary>
        /// <param name="files">Список файлов для копирования.</param>
        /// <param name="maxConcurrentTasks">Максимальное число одновременных задач.</param>
        /// <param name="session">Сессия копирования.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        private async Task ProcessFileQueueAsync(
                List<DiscoveredItem> files,
                int maxConcurrentTasks,
                CopySessionService session,
                CopyOptions? options,
                CancellationToken cancellationToken)
        {
#if DEBUG1
            int fileIndex = 0; // будем увеличивать атомарно
            _metrics?.PrepareAllFilesCopy(session.SourcePath, session.TargetPath, options?.UseMetrics ?? false);
#endif
            using var semaphore = new SemaphoreSlim(maxConcurrentTasks);
            var tasks = files.Select(file => Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    _progressTracker.StartFile(file.Source, new FileInfo(file.Source).Length);
#if DEBUG1                 
                    // Атомарный инкремент индекса файла
                    var thisFileIndex = Interlocked.Increment(ref fileIndex);
                    _copylogger.LogCopyStarted(file.Source, file.Destination);
                    var stopwatch = Stopwatch.StartNew();
#endif
                    var bufferSize = GetBufferSize(file.Source, options);
                    string category = await _categorizer.CategorizeAsync(new FileInfo(file.Source));
                    session.OnFileStarted(file.Source, file.Destination, file.FileSize, category);
                    await _fileCopier.CopyFileAsync(
                        file.Source,
                        file.Destination,
                        bufferSize,
                        bytesCopied =>
                        {
                            session.WaitIfPaused();
                            cancellationToken.ThrowIfCancellationRequested();
                            _progressTracker.UpdateProgress(bytesCopied);
                            session.UpdateFileProgress(file.Source, bytesCopied);
                            _progressReporter.Report(_progressTracker.GetProgressInfo());
#if DEBUG1
                            // Обновляем метрики/логи
                            _metrics?.OnFileCopyCompleted(file.Source, file.Destination, file.FileSize, stopwatch.Elapsed);
                            _copylogger.LogCopyCompleted(file.Source, file.Destination, file.FileSize, stopwatch.Elapsed, thisFileIndex);
#endif
                        },
                        cancellationToken,
                        session.WaitIfPaused
                    ).ConfigureAwait(false);
                    _progressTracker.CompleteFile();
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Completed);
                    session.Complete();
                }
                catch (OperationCanceledException)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
                    //_consoleOutput.WriteLine("\n[CopyManager] Операция копирования была отменена.");
                    if (_plannedItems != null)
                        session.CleanupAfterCancel(_plannedItems);
                    throw;
                }
                catch (Exception ex)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
#if DEBUG
                    _metrics?.OnError(file.Source, ex);
                    _copylogger.LogCopyError(file.Source, file.Destination, ex);
#endif
                    // Можно вызвать _errorHandler.HandleError(...)
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken)).ToList();

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Вычисляет размер буфера для копирования файла.
        /// </summary>
        /// <param name="sourcePath">Путь к исходному файлу.</param>
        /// <param name="options">Настройки копирования, содержащие минимальный и максимальный размер буфера.</param>
        /// <returns>Размер буфера в байтах.</returns>
        private int GetBufferSize(string sourcePath, CopyOptions? options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

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

        public async Task CopyFilesAsyncOld(CopySessionService session, CopyOptions options, CancellationToken cancellationToken)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrWhiteSpace(session.SourcePath) || string.IsNullOrWhiteSpace(session.TargetPath))
                return;

            // Получаем план (файлы и директории)
            var plannedItems = await _fileCopyPlanner.GetDiscoveredItems(session.SourcePath, session.TargetPath, options, cancellationToken);
            if (!plannedItems.Any())
                return;

            var files = plannedItems.OnlyFiles().ToList();
            var dirs = plannedItems.OnlyDirectories().ToList();

            // Подготовка общей информации
            long totalBytes = files.Sum(f => new FileInfo(f.Source).Length);
            int totalFiles = files.Count;

            // Инициализируем сессию (обнулим счётчики) — если в сессии есть StartSession
            session.StartSession(totalBytes, totalFiles);

            // Инициализируем трекер прогресса один раз
            _progressTracker.Start(totalBytes, totalFiles);

            // Создание общей папки назначения
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir.Source) && !Directory.Exists(dir.Destination))
                {
                    Directory.CreateDirectory(dir.Destination);
                    _metrics?.OnDirectoryCreated(dir.Destination);
                }
            }
#if DEBUG1
            int fileIndex = 0; // будем увеличивать атомарно
            _metrics?.PrepareAllFilesCopy(session.SourcePath, session.TargetPath, options?.UseMetrics ?? false);
#endif
            using var semaphore = new SemaphoreSlim(options?.MaxConсurrentTasks ?? 1);
            // Функция для безопасного запуска копирования одного файла
            var tasks = files.Select(file => Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var source = file.Source;
                    var stopwatch = Stopwatch.StartNew();

                    // Начинаем отслеживание файла
                    _progressTracker.StartFile(source, new FileInfo(source).Length);
                    int bufferSize = GetBufferSize(source, options);

                    string destinationFile;

                    if (options?.UseCategories ?? false)
                    {
                        // --- Категоризация ---
                        string category = await _categorizer.CategorizeAsync(new FileInfo(source));
                        string categoryDir = Path.Combine(session.TargetPath, category);

                        if (!Directory.Exists(categoryDir))
                            Directory.CreateDirectory(categoryDir);

                        destinationFile = Path.Combine(categoryDir, Path.GetFileName(source));
                        session.OnFileStarted(source, destinationFile, file.FileSize, category);
                    }
                    else
                    {
                        // --- Общая папка ---
                        destinationFile = Path.Combine(session.TargetPath, Path.GetFileName(source));
                        session.OnFileStarted(source, destinationFile, file.FileSize, "Default");
                    }

                    // Копирование с обработчиком прогресса
                    await _fileCopier.CopyFileAsync(
                        source,
                        destinationFile,
                        bufferSize,
                        bytesCopied =>
                        {
                            session.WaitIfPaused();
                            session.CancellationToken.ThrowIfCancellationRequested();

                            _progressTracker.UpdateProgress(bytesCopied);

                            var progressInfo = _progressTracker.GetProgressInfo();
                            try
                            {
                                session.UpdateFileProgress(file.Source, progressInfo.CurrentFileCopiedBytes);
                            }
                            catch { /* безопасно */ }

                            _progressReporter.Report(progressInfo);
                        },
                        cancellationToken,
                        session.WaitIfPaused).ConfigureAwait(false);

                    stopwatch.Stop();

                    // После завершения файла
                    _progressTracker.CompleteFile();
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Completed);

                    _progressReporter.Report(_progressTracker.GetProgressInfo());

#if DEBUG1
        _metrics?.OnFileCopyCompleted(source, destinationFile, file.FileSize, stopwatch.Elapsed);
        _copylogger.LogCopyCompleted(source, destinationFile, file.FileSize, stopwatch.Elapsed, Interlocked.Increment(ref fileIndex));
#endif
                }
                catch (OperationCanceledException)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
                    session.CleanupAfterCancel(plannedItems);
                    throw;
                }
                catch (Exception ex)
                {
                    session.UpdateFileStatus(file.Source, FileCopyStatus.Failed);
                    _metrics?.OnError(file.Source, ex);
                    _copylogger.LogCopyError(file.Source, file.Destination, ex);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken)).ToArray();

            // Финальный отчёт метрик
            _metrics?.ReportFinal();
        }
    }
}
