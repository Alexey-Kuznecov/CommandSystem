using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                    await Task.Delay(1000);
                }
            });
        }

        public DiskSpeedSnapshot StopAndExport()
        {
            _cts.Cancel();

            if (_samples == 0)
            {
                return new DiskSpeedSnapshot
                {
                    AverageMBps = 0,
                    MinMBps = 0,
                    MaxMBps = 0,
                    Samples = 0
                };
            }

            return new DiskSpeedSnapshot
            {
                AverageMBps = _totalSpeed / (double)_samples / 1024 / 1024,
                MinMBps = _minSpeed / 1024.0 / 1024.0,
                MaxMBps = _maxSpeed / 1024.0 / 1024.0,
                Samples = _samples
            };
        }

        public void Dispose()
        {
            _cts.Cancel();
            _writeCounter?.Dispose();
            _readCounter?.Dispose();
        }
    }

}
