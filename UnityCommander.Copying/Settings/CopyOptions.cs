using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Strategies;

namespace UnityCommander.Copying.Settings
{
    public class CopyOptions
    {
        public bool UseMultiThreading { get; set; }
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
        public bool UseDualChannels { get; set; } = false;
        public bool UseCategories { get; set; } = true;

        // 🔥 Новое
        public FileConflictAction ConflictResolution { get; set; } = FileConflictAction.Overwrite;
        public int RetryCount { get; set; } = 3;
        public bool ContinueOnError { get; set; } = true;
        public int BufferSize { get; set; } = 64 * 1024;     // Основной буфер
        public int MinBufferSize { get; set; } = 8 * 1024;   // Минимальный буфер для маленьких файлов
    }

    public enum FileConflictAction
    {
        Overwrite,
        Skip,
        Rename
    }
}
