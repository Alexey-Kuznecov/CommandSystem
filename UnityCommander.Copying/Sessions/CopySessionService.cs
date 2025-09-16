
using CommandSystem.Gui.MVVM;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Reporting;
using UnityCommander.SystemMetrics;

namespace UnityCommander.Copying.Sessions
{
    public class CopySessionService : ObservableObject
    {
        // --- Контроль состояния ---
        private readonly ManualResetEventSlim _pauseEvent = new(true); // для "паузы" (true = работает, Reset = стоп)
                                                                       // Change the declaration of the `_cts` field to make it mutable by removing the `readonly` modifier.
        // --- Данные о файлах ---
        private readonly List<FileCopyItem> _copiedFiles = new();       // все скопированные/копируемые файлы
        private long _totalBytes;                                       // общий размер всех файлов (счётчик внутри)
        private int _totalFiles;                                        // общее количество файлов (счётчик внутри)

        // --- Внешние зависимости ---
        private readonly ICopyFileReporter _reporter;                   // для UI/прогресса
        private readonly ICopyLogReporter _logReporter;                 // для лога (события, ошибки, детали)
        private readonly ICopySessionController _controller;

        // --- Конструктор ---
        public CopySessionService(string source, string target, ICopySessionController controller, ICopyFileReporter reporter, ICopyLogReporter logReporter)
        {
            SourcePath = source;
            TargetPath = target;
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _logReporter = logReporter;
            _controller = controller;
        }

        #region Свойства

        // --- Пути ---
        public string SourcePath { get; }       // откуда копируем
        public string TargetPath { get; }       // куда копируем

        // --- Копированные файлы ---
        public IReadOnlyList<FileCopyItem> CopiedFiles => _copiedFiles;

        // --- Настройки сессии ---
        public bool VerboseLogging { get; set; } = true; // если true — логируем каждое событие
        public int ProgressStep { get; set; }            // шаг прогресса (например, лог каждые N байт)

        // --- Прогресс ---
        public long CurrentBytesCopied { get; private set; }   // сколько байт скопировано всего
        public long BytesCopied { get; private set; }   // сколько байт скопировано всего
        public int FilesCopied { get; private set; }    // сколько файлов завершено
        public int TotalFiles { get; private set; }     // сколько всего файлов
        public long TotalBytes { get; private set; }    // сколько всего байт

        // --- Ошибки и успехи (подробности по каждому файлу) ---
        public List<FileCopyErrorContext> Errors { get; } = new();
        public List<FileCopySuccessContext> Successes { get; } = new();

        // --- CancellationToken для внешних задач ---
        public CancellationToken CancellationToken => _controller.CancellationToken;

        #endregion

        #region Управление сессией

        // запуск новой сессии
        public void StartSession(long totalBytes, int totalFiles)
        {
            TotalBytes = totalBytes;
            TotalFiles = totalFiles;
            CurrentBytesCopied = 0;
            BytesCopied = 0;
            FilesCopied = 0;
            Errors.Clear();
            Successes.Clear();

            _controller.Start(totalBytes, totalFiles);
            _logReporter.OnSessionStarted(this);
        }

        // пауза
        public void Pause()
        {
            _controller.Pause();
            _logReporter.OnSessionPaused(this);
        }

        // возобновление
        public void Resume()
        {
            _controller.Resume();
            _logReporter.OnSessionResumed(this);
        }

        // блокировка внутри копирования (ожидание, если пауза)
        public void WaitIfPaused()
        {
            _controller.WaitIfPaused();
        }

        // отмена
        public void Cancel()
        {
            _controller.Cancel();
            _logReporter.OnSessionCancelled(this);
        }

        #endregion

        #region Работа с файлами и прогрессом

        // начало копирования файла
        public void OnFileStarted(string source, string destination, long size, string category)
        {
            source = Path.GetFullPath(source);
            var item = new FileCopyItem(source, destination) 
            { 
                Size = size,
                StartTime = DateTime.Now // вот здесь фиксируем время начала копирования
            };

            lock (_copiedFiles)
                _copiedFiles.Add(item);

            _reporter.OnFileStarted(this, source, destination, size, category);
            _logReporter.OnFileStarted(this, source, destination, size, category);
        }

        // обновление прогресса по файлу
        public void UpdateFileProgress(string source, long bytesCopied)
        {
            source = Path.GetFullPath(source);
            FileCopyItem? item;

            lock (_copiedFiles)
            {
                item = _copiedFiles.FirstOrDefault(f => f.Source == source);
                if (item != null)
                {
                    item.BytesCopied = bytesCopied;
                    item.Status = FileCopyStatus.InProgress;
                    CurrentBytesCopied = _copiedFiles.Sum(f => f.BytesCopied);
                    BytesCopied += bytesCopied;
                }
            }

            if (item != null)
            {
                // вычисляем время копирования текущего файла
                var elapsed = DateTime.Now - item.StartTime;
                _totalBytes += item.BytesCopied;
                _logReporter.OnFileProgress(this, source, _totalBytes, item.Size, elapsed);
                _reporter.OnFileProgress(this, source, bytesCopied, item.Size);
            }
        }

        // завершение файла (успех или ошибка)
        public void UpdateFileStatus(string source, FileCopyStatus status)
        {
            FileCopyItem? item;
            lock (_copiedFiles)
            {
                item = _copiedFiles.FirstOrDefault(f => f.Source == source);
                if (item != null)
                {
                    item.Status = status;
                    if (status == FileCopyStatus.Completed)
                        FilesCopied++;
                }
            }

            if (item != null)
            {
                _reporter.OnFileCompleted(this, source, item.Destination, status == FileCopyStatus.Completed);

                if (status == FileCopyStatus.Completed || status == FileCopyStatus.Failed)
                {
                    var elapsed =  DateTime.Now - item.StartTime;
                    _logReporter.OnFileCompleted(this, source, item.StartTime, DateTime.Now, status == FileCopyStatus.Completed);
                   
                }
            }

            _totalBytes = 0; // ❓ тут странно — обнулять totals?
        }

        // завершение всей сессии
        public void Complete()
        {
            _controller.Complete();
            _reporter.OnSessionCompleted(this);
            _logReporter.OnSessionCompleted(this);
        }

        //public void ExportMetrics(FinalCopyReport report)
        //{
        //    _logReporter.OnSessionCompleted(this, report);
        //}

        #endregion

        #region Динамическое добавление totals

        public void AddToTotalBytes(long bytes)
        {
            Interlocked.Add(ref _totalBytes, bytes);
            TotalBytes = _totalBytes;
        }

        public void AddToTotalFiles(int count = 1)
        {
            Interlocked.Add(ref _totalFiles, count);
            TotalFiles = _totalFiles;
        }

        #endregion

        #region Очистка после отмены

        public void CleanupAfterCancel(IEnumerable<DiscoveredItem> plannedItems)
        {
            foreach (var file in plannedItems.OnlyFiles())
                TryDeleteFile(file.Destination);

            foreach (var dir in plannedItems.OnlyDirectories().OrderByDescending(d => d.Destination.Length))
                TryDeleteDirectory(dir.Destination);
        }

        private void TryDeleteFile(string path, int attempts = 3)
        {
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                    break;
                }
                catch { Thread.Sleep(50); }
            }
        }

        private void TryDeleteDirectory(string path, int attempts = 3)
        {
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    if (Directory.Exists(path))
                        Directory.Delete(path, true);
                    break;
                }
                catch { Thread.Sleep(50); }
            }
        }

        #endregion
    }
}
