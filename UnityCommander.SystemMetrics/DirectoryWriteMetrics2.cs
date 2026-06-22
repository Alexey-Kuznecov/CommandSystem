
using Timer = System.Timers.Timer;

namespace UnityCommander.SystemMetrics
{
    public class DirectoryWriteMetrics2 : IDisposable
    {
        private readonly string _path;
        private readonly Timer _reportTimer;
        private DateTime _startTime;
        private long _lastSizeBytes;

        public long TotalBytes { get; private set; }
        public event Action<long, double>? ProgressUpdated;

        public DirectoryWriteMetrics2(string path, double reportIntervalMs = 500)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Target directory '{path}' does not exist.");

            _path = path;
            _startTime = DateTime.UtcNow;
            _lastSizeBytes = GetDirectorySize(_path);

            _reportTimer = new Timer(reportIntervalMs);
            _reportTimer.Elapsed += (_, _) => RaiseProgress();
        }

        public void Start()
        {
            _startTime = DateTime.UtcNow;
            _lastSizeBytes = GetDirectorySize(_path);
            TotalBytes = 0;
            _reportTimer.Start();
        }

        public void Stop() => _reportTimer.Stop();

        private void RaiseProgress()
        {
            var currentSize = GetDirectorySize(_path);
            var delta = currentSize - _lastSizeBytes;

            if (delta > 0)
                TotalBytes += delta;

            _lastSizeBytes = currentSize;

            var totalMb = TotalBytes / 1024.0 / 1024.0;
            var seconds = (DateTime.UtcNow - _startTime).TotalSeconds;
            var speed = seconds > 0 ? totalMb / seconds : 0.0;

            ProgressUpdated?.Invoke(TotalBytes, speed);
        }

        private static long GetDirectorySize(string path)
        {
            try
            {
                return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                    .Sum(f => { try { return new FileInfo(f).Length; } catch { return 0; } });
            }
            catch
            {
                return 0;
            }
        }

        public string GetSummaryReport()
        {
            var mb = TotalBytes / 1024.0 / 1024.0;
            var elapsed = DateTime.UtcNow - _startTime;
            var speed = elapsed.TotalSeconds > 0 ? mb / elapsed.TotalSeconds : 0.0;
            return $"[DirMetrics] Total: {mb:F2} MB | Time: {elapsed.TotalSeconds:F2}s | Avg Speed: {speed:F2} MB/s";
        }

        public void Dispose()
        {
            Stop();
            _reportTimer.Dispose();
        }
    }
}
