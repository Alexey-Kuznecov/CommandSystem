
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Reporting;

namespace UnityCommander.Copying.Sessions
{
    public class CopySessionService
    {
        private readonly ManualResetEventSlim _pauseEvent = new(true);
        private readonly List<FileCopyItem> _copiedFiles = new();
        private readonly ICopyFileReporter _reporter;
        private readonly ICopyLogReporter _logReporter;
        private CancellationTokenSource? _cts;

        public CopySessionService(string source, string target, ICopyFileReporter reporter, ICopyLogReporter logReporter)
        {
            SourcePath = source;
            TargetPath = target;
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _logReporter = logReporter;
        }

        #region Свойства

        public string SourcePath { get; }
        public string TargetPath { get; }

        public IReadOnlyList<FileCopyItem> CopiedFiles => _copiedFiles;

        public long BytesCopied { get; private set; }
        public int FilesCopied { get; private set; }
        public int TotalFiles { get; private set; }
        public long TotalBytes { get; private set; }

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }

        public List<FileCopyErrorContext> Errors { get; } = new();
        public List<FileCopySuccessContext> Successes { get; } = new();

        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        #endregion

        #region Управление сессией

        public void StartSession(long totalBytes, int totalFiles)
        {
            TotalBytes = totalBytes;
            TotalFiles = totalFiles;
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

        public void Pause()
        {
            IsPaused = true;
            _pauseEvent.Reset();
            _logReporter.OnSessionPaused(this);
        }

        public void Resume()
        {
            IsPaused = false;
            _pauseEvent.Set();
            _logReporter.OnSessionResumed(this);
        }

        public void WaitIfPaused()
        {
            _pauseEvent.Wait();
            if (_cts?.Token.IsCancellationRequested ?? false)
                _cts.Token.ThrowIfCancellationRequested();
        }

        public void Cancel()
        {
            IsCancelled = true;
            _cts?.Cancel();
            _logReporter.OnSessionCancelled(this);
        }

        #endregion

        #region Работа с файлами и прогрессом

        public void OnFileStarted(string source, string destination, long size)
        {
            source = Path.GetFullPath(source); // нормализуем
            var item = new FileCopyItem(source, destination) { Size = size };
            lock (_copiedFiles)
            {
                _copiedFiles.Add(item);
            }
            _reporter.OnFileStarted(this, source, destination, size);
            _logReporter.OnFileStarted(this, source);
        }

        public void UpdateFileProgress(string source, long bytesCopied)
        {
            source = Path.GetFullPath(source); // тоже нормализуем
            FileCopyItem? item;
            lock (_copiedFiles)
            {
                item = _copiedFiles.FirstOrDefault(f => f.Source == source);
                if (item != null)
                {
                    item.BytesCopied = bytesCopied;
                    item.Status = FileCopyStatus.InProgress;
                    BytesCopied = _copiedFiles.Sum(f => f.BytesCopied);
                }
            }

            if (item != null)
            {
                _reporter.OnFileProgress(this, source, bytesCopied, item.Size);
            }
        }

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
            }

            //if (status == FileCopyStatus.Completed || status == FileCopyStatus.Failed)
                //_logReporter.OnFileCompleted(this, source, status == FileCopyStatus.Completed);
        }

        public void Complete()
        {
            IsRunning = false;
            _reporter.OnSessionCompleted(this);
            //_logReporter.OnSessionCompleted(this);
        }

        #endregion

        #region Очистка после отмены

        public void CleanupAfterCancel(IEnumerable<DiscoveredItem> plannedItems)
        {
            foreach (var file in plannedItems.OnlyFiles())
            {
                try
                {
                    if (File.Exists(file.Destination))
                        File.Delete(file.Destination);
                }
                catch
                {
                    // Игнорируем ошибки удаления отдельных файлов
                }
            }

            foreach (var dir in plannedItems.OnlyDirectories().OrderByDescending(d => d.Destination.Length))
            {
                try
                {
                    if (Directory.Exists(dir.Destination))
                        Directory.Delete(dir.Destination, false);
                }
                catch
                {
                    // Игнорируем ошибки удаления отдельных папок
                }
            }
        }

        #endregion
    }
}
