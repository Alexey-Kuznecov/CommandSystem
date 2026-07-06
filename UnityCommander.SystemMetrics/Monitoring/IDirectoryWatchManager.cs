
namespace UnityCommander.SystemMetrics.Monitoring
{
    public interface IDirectoryWatchManager
    {
        void Watch(Guid panelId, string path);

        void Unwatch(Guid panelId);

        bool IsWatching(Guid panelId);

        List<IDirectoryWatcher> GetAll();

        void StopAll();

        event EventHandler<FileSystemChangedEventArgs> FileChanged;
    }
}
