namespace UnityCommander.SystemMetrics.Monitoring
{
    public class FileSystemChangedEventArgs
    {
        public Guid Token { get; init; }

        public string FullPath { get; init; }
        
        public string? OldPath { get; init; }

        public WatcherChangeTypes ChangeType { get; init; }

        public bool IsDirectory { get; init; }

        public FileSystemChangedEventArgs(Guid token, string fullPath, WatcherChangeTypes changeTypes, string? oldPath = null)
        {
            FullPath = fullPath;
            ChangeType = changeTypes;
            OldPath = oldPath;
            Token = token;
        }
    }
}