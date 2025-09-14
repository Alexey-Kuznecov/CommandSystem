using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public class EstimatedTimeCalculator
    {
        private long _totalBytes;
        private long _bytesCopied;
        private Stopwatch _stopwatch;

        public EstimatedTimeCalculator(long totalBytes)
        {
            _totalBytes = totalBytes;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Update(long bytesCopied)
        {
            _bytesCopied = bytesCopied;
        }

        public TimeSpan GetEstimatedRemainingTime()
        {
            if (_bytesCopied == 0) return TimeSpan.MaxValue;

            var elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;
            if (elapsedSeconds < 0.1) return TimeSpan.MaxValue;

            double bytesPerSecond = _bytesCopied / elapsedSeconds;
            long remainingBytes = _totalBytes - _bytesCopied;

            if (bytesPerSecond <= 0) return TimeSpan.MaxValue;

            double estimatedSeconds = remainingBytes / bytesPerSecond;
            return TimeSpan.FromSeconds(estimatedSeconds);
        }

        internal void AddTotalBytes(long fileSize)
        {
            //throw new NotImplementedException();
        }
    }
}
