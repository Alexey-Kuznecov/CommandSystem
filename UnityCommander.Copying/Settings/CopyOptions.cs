using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Filtering;
using UnityCommander.Copying.Strategies;

namespace UnityCommander.Copying.Settings
{
    public class CopyOptions
    {
        public bool UseMultiThreading { get; set; }
        public IFileFilter? FileFilter { get; set; }
        public bool IsRecursive { get; set; }
        public bool AllowEmptyDirectories { get; set; } = true;
        public bool FlattenStructure { get; set; }
        public bool CopyAllToOneFolder { get; set; }
        public bool OverwriteExistingFiles { get; set; }
        public bool PreserveTimestamps { get; set; }
        public int MaxConсurrentTasks { get; set; } = 5;
        public IFileDiscoveryStrategy? DiscoveryStrategy { get; internal set; }
        public bool UseMetrics { get; set; }
    }
}
