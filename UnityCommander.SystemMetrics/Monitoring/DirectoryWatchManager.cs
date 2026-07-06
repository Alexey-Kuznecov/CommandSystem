
namespace UnityCommander.SystemMetrics.Monitoring
{
    public class DirectoryWatchManager : IDirectoryWatchManager
    {
        private readonly Dictionary<Guid, IDirectoryWatcher> _watchers = new();

        public event EventHandler<FileSystemChangedEventArgs>? FileChanged;

        public void Watch(Guid panelId, string path)
        {
            Unwatch(panelId);

            var watcher = new DirectoryWatcher();

            watcher.Changed += WatcherChanged;
            watcher.Tag = panelId;
            watcher.Path = path;
            watcher.Start(path);

            _watchers.Add(panelId, watcher);
        }

        public void Unwatch(Guid panelId)
        {
            if (!_watchers.TryGetValue(panelId, out var watcher))
                return;

            watcher.Changed -= WatcherChanged;
            watcher.Dispose();

            _watchers.Remove(panelId);
        }

        private void WatcherChanged(object? sender, FileSystemChangedEventArgs e)
        {
            FileChanged?.Invoke(this, e);
        }

        public void StopAll()
        {
            foreach (var watcher in _watchers.Values)
                watcher.Dispose();

            _watchers.Clear();
        }

        public bool IsWatching(Guid panelId)
            => _watchers.TryGetValue(panelId, out var watcher);

        public List<IDirectoryWatcher> GetAll() 
            => _watchers.Values.ToList();
    }
}
