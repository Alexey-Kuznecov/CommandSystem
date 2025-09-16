using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.SystemMetrics
{
    public class DiskSpeedSnapshot
    {
        public double AverageMBps { get; init; }
        public double MinMBps { get; init; }
        public double MaxMBps { get; init; }
        public int Samples { get; init; }
    }
}
