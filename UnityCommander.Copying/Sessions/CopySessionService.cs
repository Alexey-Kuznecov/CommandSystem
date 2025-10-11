
using CommandSystem.Gui.MVVM;
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

        public void StartSession(long totalBytes, int totalFiles)
        {
            _session.TotalBytes = totalBytes;
            _session.TotalFiles = totalFiles;
            _session.State = SessionState.Running;
            _session.StartTime = DateTime.Now;

            _controller.Start(totalBytes, totalFiles);
            _logReporter.OnSessionStarted(_session);
        }

        public void Pause()
        {
            _controller.Pause();
            _logReporter.OnSessionPaused(_session);
        }

        public void Resume()
        {
            _controller.Resume();
            _logReporter.OnSessionResumed(_session);
        }

        public void Cancel()
        {
            _controller.Cancel();
            _logReporter.OnSessionCancelled(_session);
        }

        public void Complete()
        {
            _controller.Complete();
            _session.EndTime = DateTime.Now;
            _uiReporter.OnSessionCompleted(_session);
            _logReporter.OnSessionCompleted(_session);
        }

        // --- Работа с файлами ---
        public void OnFileStarted(string source, string destination, long size)
        {
            var item = new FileCopyItem(source, destination)
            {
                Size = size,
                StartTime = DateTime.Now
            };

            _session.AddFile(item);

            _uiReporter.OnFileStarted(_session, source, destination, size);
            _logReporter.OnFileStarted(_session, source, destination, size); // <-- добавь это
        }

        public void UpdateFileProgress(string source, long bytesCopied)
        {
            var item = _session.GetFile(source);
            if (item == null) return;
            _session.BytesCopied += bytesCopied;
            item.BytesCopied = bytesCopied;
            item.Status = FileCopyStatus.InProgress;

            var elapsed = DateTime.Now - item.StartTime;
            _uiReporter.OnFileProgress(_session, source, item.BytesCopied, item.Size);
            _logReporter.OnFileProgress(_session, source, item.BytesCopied, item.Size); 
        }

        public void UpdateFileStatus(string source, FileCopyStatus status)
        {
            var item = _session.GetFile(source);
            if (item == null) return;

            item.Status = status;
            if (status == FileCopyStatus.Completed)
                _session.FilesCopied++;

            _uiReporter.OnFileCompleted(_session, source, item.Destination, status == FileCopyStatus.Completed);
            _logReporter.OnFileCompleted(_session, source, item.Destination, status == FileCopyStatus.Completed);
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
