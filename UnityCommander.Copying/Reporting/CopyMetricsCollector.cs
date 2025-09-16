
using System.Diagnostics;
using UnityCommander.SystemMetrics;

namespace UnityCommander.Copying.Reporting
{
    public class CopyMetricsCollector : ICopyMetricsCollector
    {
        private readonly Stopwatch _globalTimer = Stopwatch.StartNew();
        private readonly List<(string File, long Size, TimeSpan Duration)> _copiedFiles = new();
        private readonly List<(string File, Exception Error)> _errors = new();
        private int _directoriesCreated;

        // === Монитор скорости диска ===
        private DiskSpeedMonitor? _diskSpeedMonitor;
        private DirectoryWriteMetrics? _dirWatcher;

        public void PrepareAllFilesCopy(string source, string destination, bool UseMetrics)
        {
            if (UseMetrics)
            {
                _dirWatcher = new DirectoryWriteMetrics(destination);
                _dirWatcher.Start();
                var driveLetter = DiskUtils.GetPhysicalDiskInstanceFromPath(source);

                _diskSpeedMonitor = new DiskSpeedMonitor(driveLetter);
                _diskSpeedMonitor.Start(); // Запускаем мониторинг скорости диска для текущего файла
            }
        }

        public void OnFileCopyStarted(string source, string destination)
        {
        }

        public void OnFileCopyCompleted(string source, string destination, long sizeBytes, TimeSpan duration)
        {
            _copiedFiles.Add((source, sizeBytes, duration));
        }

        public void OnError(string source, Exception ex)
        {
            _errors.Add((source, ex));
        }

        public void OnDirectoryCreated(string path)
        {
            Interlocked.Increment(ref _directoriesCreated);
        }

        public CopyMetricsSnapshot GetFinalSnapshot()
        {
            var totalBytes = _copiedFiles.Sum(f => f.Size);
            var totalTime = _globalTimer.Elapsed;
            return new CopyMetricsSnapshot
            {
                TotalFiles = _copiedFiles.Count,
                TotalBytes = totalBytes,
                TotalDuration = totalTime,
                AverageSpeedBytesPerSec = totalTime.TotalSeconds > 0 ? totalBytes / totalTime.TotalSeconds : 0,
                ErrorsCount = _errors.Count
            };
        }

        public FinalCopyReport StopAndCollectReport()
        {
            var copySnapshot = this.GetFinalSnapshot();
            var diskSnapshot = _diskSpeedMonitor?.StopAndExport();
            if (_diskSpeedMonitor != null)
            {
                return new FinalCopyReport
                {
                    CopyMetrics = copySnapshot,
                    DiskMetrics = diskSnapshot
                };
            }
            return null;
        }

        public void ReportFinal()
        {
            Debug.WriteLine("");
            ReportCopySummary();
            ReportErrors();
            ReportSpeed();
            Reset();
        }

        private void ReportCopySummary()
        {
            var totalFiles = _copiedFiles.Count;
            var totalBytes = _copiedFiles.Sum(x => x.Size);
            var totalTime = _globalTimer.Elapsed;
            var avgTime = TimeSpan.FromMilliseconds(
                totalFiles > 0 ? _copiedFiles.Average(x => x.Duration.TotalMilliseconds) : 0
            );

            Debug.WriteLine($"Copied {totalFiles} files, {totalBytes / 1024.0 / 1024.0:F2} MB in {totalTime.TotalMinutes:F2} s");
            Debug.WriteLine($"Avg file time: {avgTime.TotalMilliseconds:F2} ms, Errors: {_errors.Count}, Dirs: {_directoriesCreated}");

            if (_dirWatcher != null)
            {
                var watchRepoted = _dirWatcher?.GetSummaryReport();
                _dirWatcher?.Stop();
                Debug.WriteLine(_dirWatcher?.GetSummaryReport());
            }
        }

        private void ReportErrors()
        {
            if (_errors.Count == 0)
                return;

            Debug.WriteLine("Errors:");
            foreach (var (file, ex) in _errors)
                Debug.WriteLine($" - {file}: {ex.Message}");
        }

        private void ReportSpeed()
        {
            //if (_diskSpeedMonitor == null)
            //    return;
            //_diskSpeedMonitor?.StopAndReport();
        }

        private void Reset()
        {
            _copiedFiles.Clear();
            _errors.Clear();
            _directoriesCreated = 0;
            _diskSpeedMonitor?.Dispose();
            _dirWatcher?.Dispose();
        }
    }
}
