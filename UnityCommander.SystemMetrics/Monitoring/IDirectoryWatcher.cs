
namespace UnityCommander.SystemMetrics.Monitoring
{
    public interface IDirectoryWatcher : IDisposable
    {
        event EventHandler<FileSystemChangedEventArgs> Changed;
        public Guid Tag { get; set; }
        public string? Path { get; set; }

        void Start(string path);
        void Stop();
    }
}
