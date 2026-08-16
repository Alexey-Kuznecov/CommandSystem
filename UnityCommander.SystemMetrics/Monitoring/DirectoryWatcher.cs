
namespace UnityCommander.SystemMetrics.Monitoring
{
    public class DirectoryWatcher : IDirectoryWatcher
    {
        private readonly Dictionary<string, FileSystemEntryType> _entries = new();

        public Guid Tag { get; set; }
        public string? Path { get; set; }

        private FileSystemWatcher? _watcher;

        public event EventHandler<FileSystemChangedEventArgs>? Changed;

        public void Start(string path)
        {
            Stop();

            _watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = false,
                NotifyFilter =
                    NotifyFilters.FileName |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Size
            };

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
            FileSystemEntryType type;

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    type = DefineEntryType(e.FullPath);
                    _entries[e.FullPath] = type;
                    break;

                case WatcherChangeTypes.Deleted:
                    if (!_entries.TryGetValue(e.FullPath, out type))
                        type = DefineEntryType(e.FullPath);
                    else
                        _entries.Remove(e.FullPath);
                    break;

                default:
                    if (!_entries.TryGetValue(e.FullPath, out type))
                        type = DefineEntryType(e.FullPath);
                    break;
            }

            Changed?.Invoke(this,
                new FileSystemChangedEventArgs(Tag, e.FullPath, e.ChangeType, type));
        }

        private void Renamed(object? s, RenamedEventArgs e)
        {
            if (!_entries.TryGetValue(e.OldFullPath, out var type))
            {
                // Если по какой-то причине записи нет, определяем тип заново.
                // Обычно объект уже существует под новым именем.
                type = DefineEntryType(e.FullPath);
            }
            else
            {
                _entries.Remove(e.OldFullPath);
            }

            _entries[e.FullPath] = type;

            Changed?.Invoke(this,
                new FileSystemChangedEventArgs(
                    Tag,
                    e.FullPath,
                    WatcherChangeTypes.Renamed,
                    type,
                    e.OldFullPath));
        }

        public void Dispose() => Stop();

        private FileSystemEntryType DefineEntryType(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return FileSystemEntryType.Directory;

            return (File.GetAttributes(path) & FileAttributes.Directory) != 0
                ? FileSystemEntryType.Directory
                : FileSystemEntryType.File;
        }
    }
}
