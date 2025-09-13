
using System.Collections.ObjectModel;
using UnityCommander.Copying.Sessions;

namespace UnityCommander.Copying.Reporting
{
    public class CopyFileReporter : ICopyFileReporter
    {
        private readonly ObservableCollection<FileCopyItem> _files = new();
        public ReadOnlyObservableCollection<FileCopyItem> Files { get; }
        private readonly Dictionary<string, FileCopyItem> _fileMap = new(); // быстрый доступ
        private readonly Action<Action> _invokeOnUI;

        public event Action<CopySessionService>? SessionCompleted;

        public CopyFileReporter(Action<Action> invokeOnUI)
        {
            Files = new ReadOnlyObservableCollection<FileCopyItem>(_files);
            _invokeOnUI = invokeOnUI ?? (a => a()); // по умолчанию просто выполняем
        }

        private void RunOnUI(Action action) => _invokeOnUI(action);

        public void OnFileStarted(CopySessionService session, string source, string destination, long size)
        {
            var item = new FileCopyItem(source, destination)
            {
                Size = size,
                BytesCopied = 0,
                Status = FileCopyStatus.InProgress
            };

            RunOnUI(() =>
            {
                _files.Add(item);
                _fileMap[source] = item;
            });
        }

        public void OnFileProgress(CopySessionService session, string source, long bytesCopied, long totalBytes)
        {
            if (_fileMap.TryGetValue(source, out var item))
            {
                RunOnUI(() =>
                {
                    item.BytesCopied = bytesCopied;
                    item.Status = FileCopyStatus.InProgress;
                });
            }
        }

        public void OnFileCompleted(CopySessionService session, string source, string destination, bool success)
        {
            if (_fileMap.TryGetValue(source, out var item))
            {
                RunOnUI(() =>
                {
                    item.Status = success ? FileCopyStatus.Completed : FileCopyStatus.Failed;
                    item.BytesCopied = item.Size;
                });
            }
        }

        public void OnSessionCompleted(CopySessionService session)
        {
            RunOnUI(() => SessionCompleted?.Invoke(session));
        }
    }
}
