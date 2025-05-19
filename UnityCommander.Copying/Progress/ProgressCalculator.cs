using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public class ProgressCalculator : IProgressCalculator
    {
        private double _lastProgress = 0.0;

        public double Calculate(long totalBytes, long copiedBytes, int totalFiles, int copiedFiles)
        {
            if (totalBytes <= 0)
                return 0.0;

            double rawProgress = (double)copiedBytes / totalBytes * 100.0;

            // Сглаживание: плавное приближение
            double smoothed = _lastProgress + (rawProgress - _lastProgress) * 0.3;

            _lastProgress = Math.Min(smoothed, 100.0);
            return Math.Round(_lastProgress, 1);
        }

        public double Calculate(long totalBytes, long copiedBytes)
        {
            if (totalBytes <= 0)
                return 0.0;

            double progress = (double)copiedBytes / totalBytes * 100.0;

            return progress > 100.0 ? 100.0 : progress;
        }
    }
}
