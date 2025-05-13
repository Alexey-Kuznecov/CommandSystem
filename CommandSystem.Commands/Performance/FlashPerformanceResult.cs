using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Commands.Performance
{
    public class FlashPerformanceResult
    {
        public List<double> IterationSpeedsMbPerSec { get; } = new();
        public double AverageSpeedMbPerSec => IterationSpeedsMbPerSec.Count > 0
            ? IterationSpeedsMbPerSec.Average()
            : 0;
    }
}
