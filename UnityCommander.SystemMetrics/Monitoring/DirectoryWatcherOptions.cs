
namespace UnityCommander.SystemMetrics.Monitoring
{
    public class DirectoryWatcherOptions
    {
        public string Path { get; init; }

        public bool IncludeSubdirectories { get; init; } = true;

        public NotifyFilters NotifyFilter { get; init; }

        public string Filter { get; init; } = "*";
    }
}
