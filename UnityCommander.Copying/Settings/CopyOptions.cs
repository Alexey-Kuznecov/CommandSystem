
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Strategies;

namespace UnityCommander.Copying.Settings
{
    public class CopyOptions
    {
        public bool UseMultiThreading { get; set; } = false;
        public IFileFilter? FileFilter { get; set; }
        public bool IsRecursive { get; set; }
        public bool AllowEmptyDirectories { get; set; }
        public bool FlattenStructure { get; set; }
        public bool CopyAllToOneFolder { get; set; }
        public bool OverwriteExistingFiles { get; set; }
        public bool PreserveTimestamps { get; set; }
        public int MaxConсurrentTasks { get; set; } = 5;
        public IFileDiscoveryStrategy? DiscoveryStrategy { get; set; }
        public bool UseMetrics { get; set; }
        public bool UseDualChannels { get; set; } = true;
        public bool UseCategories { get; set; } = true;

        // 🔥 Новое
        public FileConflictAction ConflictResolution { get; set; } = FileConflictAction.Overwrite;
        public int RetryCount { get; set; } = 3;
        public bool ContinueOnError { get; set; } = true;
        public int BufferSize { get; set; } = 64 * 1024;     // Основной буфер
        public int MinBufferSize { get; set; } = 8 * 1024;   // Минимальный буфер для маленьких файлов
        public bool UseProgressiveDiscovery { get; set; }
        public bool VerboseLogging { get; set; }
        public bool UseWinApi { get; set; }
        public bool UseParallel { get; set; }
    }

    public enum FileConflictAction
    {
        Overwrite,
        Skip,
        Rename
    }
}
