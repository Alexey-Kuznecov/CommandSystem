namespace UnityCommander.SystemMetrics.Monitoring
{
    public interface IFileSystemEventSource
    {
        event EventHandler<FileSystemChangedEventArgs> Changed;
    }
}
