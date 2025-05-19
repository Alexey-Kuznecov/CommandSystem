using System.Diagnostics;

namespace UnityCommander.SystemMetrics
{
    public class DiskSpeedMonitor : IDisposable
    {
        private readonly PerformanceCounter _writeCounter;
        private readonly PerformanceCounter _readCounter;
        private readonly CancellationTokenSource _cts = new();
        
        private long _minSpeed = long.MaxValue;
        private long _maxSpeed = 0;
        private long _totalSpeed = 0;
        private int _samples = 0;

        public DiskSpeedMonitor(string instanceName = "_Total")
        {
            _writeCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", instanceName);
            _readCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", instanceName);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                _writeCounter.NextValue(); // инициализация
                await Task.Delay(1000);

                while (!_cts.IsCancellationRequested)
                {
                    float writeBytesPerSec = _writeCounter.NextValue();
                    long speed = (long)writeBytesPerSec;

                    if (speed < _minSpeed) _minSpeed = speed;
                    if (speed > _maxSpeed) _maxSpeed = speed;

                    _totalSpeed += speed;
                    _samples++;

                    //Console.WriteLine($"[DiskSpeed] Write: {speed / 1024.0 / 1024.0:F2} MB/s");
                    await Task.Delay(1000);
                }
            });
        }

        public void StopAndReport()
        {
            _cts.Cancel();
            if (_samples == 0)
            {
                Console.WriteLine("[DiskSpeed] No samples collected.");
                return;
            }

            Console.WriteLine("[DiskSpeed] Final Report:");
            Console.WriteLine($" - Avg: {_totalSpeed / _samples / 1024.0 / 1024.0:F2} MB/s");
            Console.WriteLine($" - Min: {_minSpeed / 1024.0 / 1024.0:F2} MB/s");
            Console.WriteLine($" - Max: {_maxSpeed / 1024.0 / 1024.0:F2} MB/s");
        }

        public void Dispose()
        {
            _cts.Cancel();
            _writeCounter?.Dispose();
            _readCounter?.Dispose();
        }
    }
}
