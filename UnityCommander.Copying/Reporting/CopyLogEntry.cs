
namespace UnityCommander.Copying.Reporting
{
    public class CopyLogEntry
    {
        public object? Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Message { get; set; }
        public string? FilePath { get; set; }
        public Exception? Exception { get; set; }
    }
}