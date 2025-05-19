
namespace UnityCommander.Copying.Core
{
    public class ProgressInfo
    {
        public string? CurrentFile { get; set; }
        public long TotalBytes { get; set; }
        public long BytesCopied { get; set; }
        public int TotalFiles { get; set; }
        public int FilesCopied { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public double? SpeedMBps { get; set; }
        public double CompletionPercentage { get; set; }
        public double SpeedBytesPerSecond { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }
    }
}
