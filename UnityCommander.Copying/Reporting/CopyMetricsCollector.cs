using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Reporting
{
    public class CopyMetricsCollector : ICopyMetricsCollector
    {
        private readonly Stopwatch _globalTimer = Stopwatch.StartNew();
        private readonly List<(string File, long Size, TimeSpan Duration)> _copiedFiles = new();
        private readonly List<(string File, Exception Error)> _errors = new();
        private int _directoriesCreated;

        public void OnFileCopyStarted(string source, string destination) { /* optional */ }

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

        public void ReportFinal()
        {
            var totalFiles = _copiedFiles.Count;
            var totalBytes = _copiedFiles.Sum(x => x.Size);
            var totalTime = _globalTimer.Elapsed;
            var avgTime = TimeSpan.FromMilliseconds(_copiedFiles.Any() ? _copiedFiles.Average(x => x.Duration.TotalMilliseconds) : 0);

            Console.WriteLine($"Copied {totalFiles} files, {totalBytes / 1024.0 / 1024.0:F2} MB in {totalTime.TotalSeconds:F2} s");
            Console.WriteLine($"Avg file time: {avgTime.TotalMilliseconds:F2} ms, Errors: {_errors.Count}, Dirs: {_directoriesCreated}");

            if (_errors.Count > 0)
            {
                Console.WriteLine("Errors:");
                foreach (var (file, ex) in _errors)
                    Console.WriteLine($" - {file}: {ex.Message}");
            }
        }
    }

}
