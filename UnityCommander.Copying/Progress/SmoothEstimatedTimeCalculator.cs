using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public class SmoothEstimatedTimeCalculator
    {
        private readonly long _totalBytes;
        private long _bytesCopied;

        private readonly Queue<(DateTime time, long bytesCopied)> _samples = new();
        private readonly TimeSpan _sampleWindow = TimeSpan.FromSeconds(5);

        private TimeSpan? _lastEstimate;

        public SmoothEstimatedTimeCalculator(long totalBytes)
        {
            _totalBytes = totalBytes;
        }

        public void Update(long bytesCopied)
        {
            _bytesCopied = bytesCopied;
            var now = DateTime.UtcNow;

            _samples.Enqueue((now, bytesCopied));

            // Удаляем старые точки
            while (_samples.Count > 1 && now - _samples.Peek().time > _sampleWindow)
                _samples.Dequeue();
        }

        public TimeSpan GetEstimatedRemainingTime()
        {
            if (_samples.Count < 2) return _lastEstimate ?? TimeSpan.MaxValue;

            var (oldestTime, oldestBytes) = _samples.Peek();
            var (latestTime, latestBytes) = _samples.Last();

            var duration = (latestTime - oldestTime).TotalSeconds;
            var bytesDelta = latestBytes - oldestBytes;

            if (duration <= 0 || bytesDelta <= 0)
                return _lastEstimate ?? TimeSpan.MaxValue;

            double speed = bytesDelta / duration; // bytes per second
            long remaining = _totalBytes - _bytesCopied;
            double estimatedSec = remaining / speed;

            var estimate = TimeSpan.FromSeconds(estimatedSec);

            // Сглаживание с предыдущим значением
            if (_lastEstimate.HasValue)
            {
                estimate = TimeSpan.FromSeconds((_lastEstimate.Value.TotalSeconds * 0.7) + (estimate.TotalSeconds * 0.3));
            }

            _lastEstimate = estimate;
            return estimate;
        }
    }
}
