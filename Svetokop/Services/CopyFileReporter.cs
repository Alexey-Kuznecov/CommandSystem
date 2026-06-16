
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Windows.Threading;
using System.Windows;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;

namespace Svetokop.Services
{
    public class CopyFileReporter : ICopyReporter
    {
        private readonly ObservableCollection<FileCopyItem> _files = new();
        public ObservableCollection<FileCopyItem> Files { get; }

        private readonly Dictionary<string, FileCopyItem> _fileMap = new();
        private readonly ConcurrentQueue<FileCopyItem> _updateQueue = new();
        private readonly Dispatcher _dispatcher;
        private readonly DispatcherTimer _updateTimer;

        public event Action? FilesChanged;

        public CopyFileReporter()
        {
            //Files = new ObservableCollection<FileCopyItem>(_files);
            //_dispatcher = Application.Current.Dispatcher;

            //_updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            //_updateTimer.Tick += (s, e) => ProcessPendingUpdates();
            //_updateTimer.Start();
        }

        // Предзагрузка всех файлов
        public void PrepareFileList(IEnumerable<(string source, string destination, long size)> files)
        {
            //foreach (var f in files)
            //{
            //    var item = new FileCopyItem(f.source, f.destination, f.size);
            //    _files.Add(item);
            //    _fileMap[f.source] = item;
            //}
            //FilesChanged?.Invoke();
        }

        public void OnFileProgress(string source, long bytesCopied)
        {
            //if (_fileMap.TryGetValue(source, out var item))
            //{
            //    item.BytesCopied += bytesCopied;
            //    item.Status = FileCopyStatus.InProgress;
            //    _updateQueue.Enqueue(item); // обновление батчами через Dispatcher
            //}
        }

        public void OnFileCompleted(string source, bool success)
        {
            //if (_fileMap.TryGetValue(source, out var item))
            //{
            //    item.BytesCopied = item.Size;
            //    item.Status = success ? FileCopyStatus.Completed : FileCopyStatus.Failed;
            //    _updateQueue.Enqueue(item);
            //}
        }

        private void ProcessPendingUpdates()
        {
            int count = 0;
            while (count < 50 && _updateQueue.TryDequeue(out var item))
            {
                item.UpdateDisplayValues();
                count++;
            }
            if (count > 0) FilesChanged?.Invoke();
        }

        public void OnFileStarted(CopySession session, string source, string destination, long size)
        {
            //throw new NotImplementedException();
        }
    }
}
