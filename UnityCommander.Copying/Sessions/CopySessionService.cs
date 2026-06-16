
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Reporting;

namespace UnityCommander.Copying.Sessions
{
    public class CopySessionService
    {
        private readonly CopySession _session;
        private readonly ICopySessionController _controller;
        private readonly ICopyReporter _uiReporter;
        private readonly ICopyReporter _logReporter;

        public CopySessionService(CopySession session, ICopySessionController controller, ICopyReporter uiReporter, ICopyReporter logReporter)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _uiReporter = uiReporter ?? throw new ArgumentNullException(nameof(uiReporter));
            _logReporter = logReporter ?? throw new ArgumentNullException(nameof(logReporter));
        }

        public CancellationToken CancellationToken => _controller.CancellationToken;

        public ICopySessionController Controller => _controller;
        public CopySession CurrentSession => _session;

        public ICopyReporter FileReporter => _uiReporter;

        public ICopyReporter LogReporter => _logReporter;

        public long SmallFileThreshold { get; } = 1048576;

        public void StartSession(long totalBytes, int totalFiles)
        {
            _session.TotalBytes = totalBytes;
            _session.TotalFiles = totalFiles;
            _session.State = SessionState.Running;
            _session.StartTime = DateTime.Now;
            _controller.Start(totalBytes, totalFiles);
            //_logReporter.OnSessionStarted(_session);
        }

        public void Complete()
        {
            _controller.Complete();
            _session.EndTime = DateTime.Now;
            //_uiReporter.OnSessionCompleted(_session);
            //_logReporter.OnSessionCompleted(_session);
        }

        public void Pause()
        {
            _controller.Pause();
            //_logReporter.OnSessionPaused(_session);
        }

        public void Resume()
        {
            _controller.Resume();
            //_logReporter.OnSessionResumed(_session);
        }

        public void Cancel()
        {
            _controller.Cancel();
            //_logReporter.OnSessionCancelled(_session);
        }

        public void PrepareFileList(IEnumerable<(string source, string destination, long size)> files)
        {
            _uiReporter.PrepareFileList(files);
        }


        // --- Работа с файлами ---
        public void OnFileStarted(string source, string destination, long size)
        {
            var item = new FileCopyItem(source, destination, size)
            {
                Size = size,
                StartTime = DateTime.Now
            };

            _session.AddFile(item);

            // Если файл маленький, сразу помечаем Completed
            //if (size <= SmallFileThreshold)
            //{
            //    item.BytesCopied = size;
            //    item.Status = FileCopyStatus.Completed;
            //    item.Progress = 100;

            //    if (CurrentSession.VerboseLogging)
            //    {
            //        _uiReporter.OnFileCompleted(source, true);
            //        _logReporter.OnFileCompleted(source, true);
            //    }
            //}
            //else
            //{
                if (CurrentSession.VerboseLogging)
                {
                    _uiReporter.OnFileStarted(_session, source, destination, size);
                    _logReporter.OnFileStarted(_session, source, destination, size);
                }
            //}
        }

        public void UpdateFileProgress(string source, long bytesCopied)
        {
            var item = _session.GetFile(source);
            if (item == null) return;

            // Пропускаем маленькие файлы, уже помеченные как Completed
            if (item.Status == FileCopyStatus.Completed)
                return;

            _session.BytesCopied += bytesCopied;
            item.BytesCopied += bytesCopied; // добавляем, а не перезаписываем
            item.Status = FileCopyStatus.InProgress;

            if (CurrentSession.VerboseLogging)
            {
                _uiReporter.OnFileProgress(source, item.BytesCopied);
                //_logReporter.OnFileProgress(_session, source, item.BytesCopied, item.Size);
            }
        }

        public void UpdateFileStatus(string source, FileCopyStatus status)
        {
            var item = _session.GetFile(source);
            if (item == null) return;

            // Если файл уже Completed (малый), пропускаем
            if (item.Status == FileCopyStatus.Completed)
                return;

            item.Status = status;
            if (status == FileCopyStatus.Completed)
                _session.FilesCopied++;

            if (CurrentSession.VerboseLogging)
            {
                _uiReporter.OnFileCompleted(source, status == FileCopyStatus.Completed);
                //_logReporter.OnFileCompleted(_session, source, item.Destination, status == FileCopyStatus.Completed);
            }
        }

        public void CleanupAfterCancel(IEnumerable<DiscoveredItem> plannedItems)
        {
            foreach (var file in plannedItems.OnlyFiles())
                TryDeleteFile(file.Destination);

            foreach (var dir in plannedItems.OnlyDirectories().OrderByDescending(d => d.Destination.Length))
                TryDeleteDirectory(dir.Destination);
        }

        private void TryDeleteFile(string path, int attempts = 3) { /* твоя реализация */ }
        private void TryDeleteDirectory(string path, int attempts = 3) { /* твоя реализация */ }

        internal void AddToTotalFiles(int v)
        {
            throw new NotImplementedException();
        }

        internal void AddToTotalBytes(long fileSize)
        {
            throw new NotImplementedException();
        }
    }
}
