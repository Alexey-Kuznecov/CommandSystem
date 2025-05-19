using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UnityCommander.SystemMetrics
{
    public class DirectoryWriteMetrics : IDisposable
    {
        private readonly string _path;
        private readonly FileSystemWatcher _watcher;
        private readonly Timer _reportTimer;
        private readonly HashSet<string> _trackedFiles = new(StringComparer.OrdinalIgnoreCase);

        private DateTime _startTime;
        private long _lastSizeBytes;
        private bool _isRunning;

        public long TotalBytes { get; private set; }
        public event Action<long, double>? ProgressUpdated;

        public DirectoryWriteMetrics(string path, double reportIntervalMs = 1000)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Target directory '{path}' does not exist.");

            _path = path;
            _startTime = DateTime.UtcNow;
            _lastSizeBytes = GetDirectorySize(_path);

            _watcher = new FileSystemWatcher(_path)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = false // запуск только в Start()
            };

            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;
            _watcher.Renamed += OnChanged;

            _reportTimer = new Timer(reportIntervalMs);
            _reportTimer.Elapsed += (_, _) => RaiseProgress();
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;

            _startTime = DateTime.UtcNow;
            _lastSizeBytes = GetDirectorySize(_path);
            TotalBytes = 0;

            _watcher.EnableRaisingEvents = true;
            _reportTimer.Start();
        }

        public void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;

            _watcher.EnableRaisingEvents = false;
            _reportTimer.Stop();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var currentSize = GetDirectorySize(_path);
                var delta = currentSize - _lastSizeBytes;

                if (delta > 0)
                    TotalBytes += delta;

                _lastSizeBytes = currentSize;
            }
            catch
            {
                // Игнорируем ошибки доступа или гонки
            }
        }

        private static long GetDirectorySize(string path)
        {
            try
            {
                return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                    .Sum(f =>
                    {
                        try { return new FileInfo(f).Length; }
                        catch { return 0; } // на случай доступа к заблокированным файлам
                    });
            }
            catch
            {
                return 0;
            }
        }

        private void RaiseProgress()
        {
            var totalMb = TotalBytes / 1024.0 / 1024.0;
            var seconds = (DateTime.UtcNow - _startTime).TotalSeconds;
            var speed = seconds > 0 ? totalMb / seconds : 0.0;

            ProgressUpdated?.Invoke(TotalBytes, speed);
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
            _watcher.Dispose();
            _reportTimer.Dispose();
            _trackedFiles.Clear();
        }
    }
}
