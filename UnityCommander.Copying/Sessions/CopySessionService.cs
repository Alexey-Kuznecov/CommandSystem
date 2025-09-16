
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
        private CancellationTokenSource? _cts; // Removed 'readonly' to allow reassignment

        // --- Данные о файлах ---
        private readonly List<FileCopyItem> _copiedFiles = new();       // все скопированные/копируемые файлы
        private long _totalBytes;                                       // общий размер всех файлов (счётчик внутри)
        private int _totalFiles;                                        // общее количество файлов (счётчик внутри)

        // --- Внешние зависимости ---
        private readonly ICopyFileReporter _reporter;                   // для UI/прогресса
        private readonly ICopyLogReporter _logReporter;                 // для лога (события, ошибки, детали)

        // --- Текущее состояние сессии ---
        private SessionState _state;
        public event EventHandler<SessionState>? StateChanged;          // UI/ViewModel могут подписаться

        // --- Конструктор ---
        public CopySessionService(string source, string target, ICopyFileReporter reporter, ICopyLogReporter logReporter)
        {
            SourcePath = source;
            TargetPath = target;
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _logReporter = logReporter;
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

        // --- Флаги состояния ---
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }

        // --- Текущее состояние ---
        public SessionState State
        {
            get => _state;
            private set
            {
                if (SetProperty(ref _state, value))
                    StateChanged?.Invoke(this, _state);
            }
        }

        // --- Ошибки и успехи (подробности по каждому файлу) ---
        public List<FileCopyErrorContext> Errors { get; } = new();
        public List<FileCopySuccessContext> Successes { get; } = new();

        // --- CancellationToken для внешних задач ---
        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        #endregion

        #region Управление сессией

        // запуск новой сессии
        public void StartSession(long totalBytes, int totalFiles)
        {
            State = SessionState.Running;
            TotalBytes = totalBytes;
            TotalFiles = totalFiles;
            CurrentBytesCopied = 0;
            BytesCopied = 0;
            FilesCopied = 0;
            Errors.Clear();
            Successes.Clear();

            IsRunning = true;
            IsPaused = false;
            IsCancelled = false;

            _cts = new CancellationTokenSource();
            _logReporter.OnSessionStarted(this);
        }

        // пауза
        public void Pause()
        {
            State = SessionState.Paused;
            IsPaused = true;
            _pauseEvent.Reset(); // стоп
            _logReporter.OnSessionPaused(this);
        }

        // возобновление
        public void Resume()
        {
            State = SessionState.Running;
            IsPaused = false;
            _pauseEvent.Set(); // продолжение
            _logReporter.OnSessionResumed(this);
        }

        // блокировка внутри копирования (ожидание, если пауза)
        public void WaitIfPaused()
        {
            _pauseEvent.Wait();
            if (_cts?.Token.IsCancellationRequested ?? false)
                _cts.Token.ThrowIfCancellationRequested();
        }

        // отмена
        public void Cancel()
        {
            State = SessionState.Completed; // ⬅️ тут может путаница: Completed vs Cancelled
            IsCancelled = true;
            _cts?.Cancel();
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
            State = SessionState.Cancelled;
            IsRunning = false;
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
