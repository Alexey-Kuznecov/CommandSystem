using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.SystemMetrics
{
    public class CopyMetricsSnapshot
    {
        public int TotalFiles { get; set; }
        public long TotalBytes { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public double AverageSpeedBytesPerSec { get; set; }
        public int ErrorsCount { get; set; }
    }
}
