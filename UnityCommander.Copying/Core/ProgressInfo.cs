
namespace UnityCommander.Copying.Core
{
    public class ProgressInfo
    {
        public string CurrentFile { get; set; }
        public long TotalBytes { get; set; }
        public long BytesCopied { get; set; }
        public int TotalFiles { get; set; }
        public int FilesCopied { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public double? SpeedMBps { get; set; } // MB/s

        public double CompletionPercentage { get; set; }
        public TimeSpan? EstimatedTimeRemaining
        {
            get
            {
                if (SpeedMBps.HasValue && SpeedMBps.Value > 0 && TotalBytes > 0)
                {
                    long remainingBytes = TotalBytes - BytesCopied;
                    double remainingSeconds = (remainingBytes / 1024.0 / 1024.0) / SpeedMBps.Value;
                    return TimeSpan.FromSeconds(remainingSeconds);
                }
                return null;
            }
        }
    }
}
