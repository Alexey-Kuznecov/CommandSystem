using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Core
{
    public enum DiscoveredItemType
    {
        File,
        Directory
    }

    public class DiscoveredItem
    {
        public string Source { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public long FileSize { get; set; } = 0;
        public FileInfo? FileInfo { get; set; }
        public DiscoveredItemType Type { get; set; }
        public bool HasFilesInside { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
