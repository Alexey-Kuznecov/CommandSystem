
namespace UnityCommander.SystemMetrics.Monitoring
{
    public class DirectoryWatcher : IDirectoryWatcher
    {
        public Guid Tag { get; set; }
        public string? Path { get; set; }

        private FileSystemWatcher? _watcher;

        public event EventHandler<FileSystemChangedEventArgs>? Changed;

        public void Start(string path)
        {
            Stop();

            _watcher = new FileSystemWatcher(path);

            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.DirectoryName |
                NotifyFilters.LastWrite |
                NotifyFilters.Size;

            _watcher.Created += Raise;
            _watcher.Changed += Raise;
            _watcher.Deleted += Raise;
            _watcher.Renamed += Renamed;

            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher?.Dispose();
            _watcher = null;
        }

        private void Raise(object? s, FileSystemEventArgs e)
        {
            Changed?.Invoke(this,
                new FileSystemChangedEventArgs(Tag, e.FullPath, e.ChangeType));
        }

        private void Renamed(object? s, RenamedEventArgs e)
        {
            Changed?.Invoke(this,
                new FileSystemChangedEventArgs(
                    Tag,
                    e.FullPath,
                    WatcherChangeTypes.Renamed,
                    e.OldFullPath));
        }

        public void Dispose() => Stop();
    }
}
