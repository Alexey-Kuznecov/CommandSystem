
using AlexeyKuznetsov.Helper;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;

namespace Svetokop.Services
{
    public class CopyFileReporter2 : ICopyReporter
    {
        private readonly ObservableCollection<FileCopyItem> _files = new();
        public ReadOnlyObservableCollection<FileCopyItem> Files { get; }

        public event Action? FilesChanged; // новое событие
        private readonly Dictionary<string, FileCopyItem> _fileMap = new(); // быстрый доступ
        private readonly Dispatcher _dispatcher;

        public event Action<CopySession>? SessionCompleted;

        public CopyFileReporter2()
        {
            Files = new ReadOnlyObservableCollection<FileCopyItem>(_files);
            _dispatcher = Application.Current.Dispatcher;
        }

        private void RunOnUI(Action action)
        {
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.Invoke(action);
        }

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
                FilesChanged?.Invoke();
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
                    item.FileName = Path.GetFileName(source);
                    item.ProgressText = $"{item.Progress}%";
                    item.FileSizeText = FastBytesFarmater.FormatSize(item.Size);
                    item.BytesCopiedText = FastBytesFarmater.FormatSize(item.BytesCopied);
                    item.Status = FileCopyStatus.InProgress;
                    FilesChanged?.Invoke();
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
                    FilesChanged?.Invoke();
                });
            }
        }

        public void OnSessionStarted(CopySession session)
        {
            // Можно добавить логику, если нужно
        }

        public void OnSessionCompleted(CopySession session)
        {
            RunOnUI(() => SessionCompleted?.Invoke(session));
        }

        public void OnFileCategorized(CopySession session, string source, string category)
        {
            // Пока не реализовано
        }

        public void OnSessionPaused(CopySession session) { }
        public void OnSessionResumed(CopySession session) { }
        public void OnSessionCancelled(CopySession session) { }
    }
}
