using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public class SpeedCalculator : ISpeedCalculator
    {
        private readonly TimeSpan _windowSize = TimeSpan.FromSeconds(10); // можно поэкспериментировать с 2–5 сек
        private readonly Queue<(DateTime Time, long Bytes)> _samples = new();
        private long _lastTotalBytes;
        private DateTime _lastSpeedCalculationTime = DateTime.MinValue;
        private double _lastSpeed;

        public void Reset()
        {
            lock (_samples)
            {
                _samples.Clear();
                _lastTotalBytes = 0;
            }
        }

        public void Update(long currentTotalBytes)
        {
            var now = DateTime.UtcNow;
            var delta = currentTotalBytes - _lastTotalBytes;
            _lastTotalBytes = currentTotalBytes;

            lock (_samples)
            {
                _samples.Enqueue((now, delta));

                // Убираем старые точки, за пределами окна
                while (_samples.Count > 0 && (now - _samples.Peek().Time) > _windowSize)
                {
                    _samples.Dequeue();
                }
            }
        }

        public double GetSpeedBytesPerSecond()
        {
            var now = DateTime.UtcNow;
            if ((now - _lastSpeedCalculationTime).TotalMilliseconds < 500)
                return _lastSpeed;

            _lastSpeedCalculationTime = now;

            lock (_samples)
            {
                if (_samples.Count < 2)
                    return 0;

                var first = _samples.Peek();
                var last = _samples.Last();

                var totalBytes = _samples.Sum(s => s.Bytes);
                var seconds = (last.Time - first.Time).TotalSeconds;

                _lastSpeed = (seconds <= 0) ? 0 : totalBytes / seconds;
                return _lastSpeed;
            }
        }
    }
}
