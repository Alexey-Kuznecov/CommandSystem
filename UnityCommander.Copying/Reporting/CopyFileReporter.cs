
using AlexeyKuznetsov.Helper;
using System.Collections.ObjectModel;
using UnityCommander.Copying.Sessions;

namespace UnityCommander.Copying.Reporting
{
    public class CopyFileReporter : ICopyReporter
    {
        private readonly ObservableCollection<FileCopyItem> _files = new();
        public ReadOnlyObservableCollection<FileCopyItem> Files { get; }
        private readonly Dictionary<string, FileCopyItem> _fileMap = new(); // быстрый доступ
        private readonly Action<Action> _invokeOnUI;

        public event Action<CopySession>? SessionCompleted;

        public CopyFileReporter(Action<Action> invokeOnUI)
        {
            Files = new ReadOnlyObservableCollection<FileCopyItem>(_files);
            _invokeOnUI = invokeOnUI ?? (a => a()); // по умолчанию просто выполняем
        }

        private void RunOnUI(Action action) => _invokeOnUI(action);

        public void OnFileStarted(CopySession session, string source, string destination, long size)
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

        public void OnFileProgress(CopySession session, string source, long bytesCopied, long totalBytes)
        {
            if (_fileMap.TryGetValue(source, out var item))
            {
                item.BytesCopied += bytesCopied; // безопасное обновление
                item.Progress = item.Size > 0
                    ? (int)Math.Round((double)item.BytesCopied / item.Size * 100)
                    : 0;

                RunOnUI(() =>
                {
                    item.FileSizeText = FastBytesFarmater.FormatSize(item.Size);
                    item.BytesCopiedText = FastBytesFarmater.FormatSize(item.BytesCopied);
                    item.Status = FileCopyStatus.InProgress;
                });
            }
        }


        public void OnFileCompleted(CopySession session, string source, string destination, bool success)
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

        public void OnSessionStarted(CopySession session)
        {
            throw new NotImplementedException();
        }

        public void OnSessionCompleted(CopySession session)
        {
            RunOnUI(() => SessionCompleted?.Invoke(session));
        }

        public void OnFileCategorized(CopySession session, string source, string category)
        {
            //_reporter.OnFileCategorized(session, source, category);
        }

        public void OnSessionPaused(CopySession session)
        {
            throw new NotImplementedException();
        }

        public void OnSessionResumed(CopySession session)
        {
            throw new NotImplementedException();
        }

        public void OnSessionCancelled(CopySession session)
        {
            throw new NotImplementedException();
        }
    }
}
