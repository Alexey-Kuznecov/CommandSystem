namespace UnityCommander.SystemMetrics.Monitoring
{
    public interface IFileSystemMonitor
    {
        void Start(Guid ownerId, string path);

        void Stop(Guid ownerId);

        void ChangePath(Guid ownerId, string path);

        event EventHandler<FileSystemChangedEventArgs> Changed;
    }
}
