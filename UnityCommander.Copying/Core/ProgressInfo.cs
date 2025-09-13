
namespace UnityCommander.Copying.Core
{
    public class ProgressInfo
    {
        public string? CurrentFilePath { get; set; }
        public long CurrentFileSize { get; set; }      // общий размер текущего файла
        public long CurrentFileCopiedBytes { get; set; } // сколько уже скопировано из него
        public long TotalBytes { get; set; }
        public long BytesCopied { get; set; }
        public int TotalFiles { get; set; }
        public int FilesCopied { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public double CompletionPercentage { get; set; }
        public double SpeedBytesPerSecond { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }

        // --- новые удобные вычисляемые свойства ---
        public int CurrentFilePercentage =>
            CurrentFileSize > 0
                ? (int)Math.Min(100, Math.Round((double)CurrentFileCopiedBytes / CurrentFileSize * 100))
                : 0;

        public string CurrentFileProgressText =>
            CurrentFileSize > 0
                ? $"{CurrentFilePercentage}% ({Math.Min(CurrentFileCopiedBytes, CurrentFileSize) / 1024.0 / 1024.0:F2} MB of {CurrentFileSize / 1024.0 / 1024.0:F2} MB)"
                : string.Empty;

        public string TotalProgressText =>
            $"{CompletionPercentage:F2}% ({BytesCopied / 1024.0 / 1024.0:F2} MB of {TotalBytes / 1024.0 / 1024.0:F2} MB)";
    }
}
