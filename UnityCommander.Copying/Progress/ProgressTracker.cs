
using System;
using System.Diagnostics;
using UnityCommander.Copying.Core;
using UnityCommander.SystemMetrics;

namespace UnityCommander.Copying.Progress
{
    public class ProgressTracker : IProgressTracker
    {
        private readonly IProgressCalculator _progressCalculator;

        private long _totalBytes;
        private int _totalFiles;
        private long _bytesCopied;
        private int _filesCopied;
        private ProgressInfo _progressInfo;
        private readonly ISpeedCalculator? _speedCalculator;
        private Stopwatch? _elapsedTimePerFile;
        private EstimatedTimeCalculator? _timeCalculator;

        private readonly object _progressLock = new();
        private string? _currentFilePath;
        private long _currentFileBytesCopied;

        public ProgressTracker(IProgressCalculator progressCalculator, ISpeedCalculator? speedCalculator = null)
        {
            _progressCalculator = progressCalculator ?? new ProgressCalculator();
            _speedCalculator = speedCalculator;
            _progressInfo = new ProgressInfo();

            Debug.WriteLine("[ProgressTracker] Создан экземпляр");
        }

        public void Start(long totalBytes, int totalFiles)
        {
            Debug.WriteLine($"[Start] Инициализация: TotalBytes={totalBytes}, TotalFiles={totalFiles}");

            _totalBytes = totalBytes;
            _totalFiles = totalFiles;
            _bytesCopied = 0;
            _filesCopied = 0;
            _progressInfo = new ProgressInfo
            {
                TotalFiles = totalFiles,
                TotalBytes = totalBytes
            };

            _speedCalculator?.Reset();
            _timeCalculator = new EstimatedTimeCalculator(totalBytes);
        }

        public void StartFile(string sourcePath, long size)
        {
            Debug.WriteLine($"[StartFile] Начало файла: {Path.GetFileName(sourcePath)}, Size={size}");

            _elapsedTimePerFile = Stopwatch.StartNew();
            lock (_progressLock)
            {
                _currentFilePath = sourcePath;
                _currentFileBytesCopied = 0;
                _progressInfo.CurrentFilePath = sourcePath;
                _progressInfo.CurrentFileSize = size;
                _progressInfo.CurrentFileCopiedBytes = 0;
            }
        }

        public void UpdateProgress(long bytesCopied)
        {
            long currentBytes = Interlocked.Add(ref _bytesCopied, bytesCopied);
            _progressInfo.BytesCopied = currentBytes;

            lock (_progressLock)
            {
                if (_currentFilePath != null)
                {
                    _currentFileBytesCopied += bytesCopied;
                    _progressInfo.CurrentFileCopiedBytes = _currentFileBytesCopied;

                    Debug.WriteLine(
                        $"[UpdateProgress] Файл={Path.GetFileName(_currentFilePath)}, " +
                        $"Copied={_currentFileBytesCopied}/{_progressInfo.CurrentFileSize}, " +
                        $"Global={currentBytes}/{_totalBytes}");
                }
            }

            if (_speedCalculator != null)
            {
                _speedCalculator.Update(currentBytes);
                _progressInfo.SpeedBytesPerSecond = _speedCalculator.GetSpeedBytesPerSecond();
            }

            _timeCalculator?.Update(_bytesCopied);
            var remaining = _timeCalculator?.GetEstimatedRemainingTime();
            if (remaining.HasValue)
                _progressInfo.EstimatedTimeRemaining = remaining.Value;

            _progressInfo.CompletionPercentage = _progressCalculator.Calculate(_totalBytes, _bytesCopied);
            _progressInfo.FilesCopied = _filesCopied;
            _progressInfo.TotalFiles = _totalFiles;
        }

        public void CompleteFile()
        {
            _elapsedTimePerFile?.Stop();
            Interlocked.Increment(ref _filesCopied);

            Debug.WriteLine($"[CompleteFile] Файл завершён. Всего файлов={_filesCopied}/{_totalFiles}");

            lock (_progressLock)
            {
                _progressInfo.CurrentFileCopiedBytes = _currentFileBytesCopied;
                _currentFilePath = null;
                _currentFileBytesCopied = 0;
            }

            _progressInfo.FilesCopied = _filesCopied;
            _progressInfo.ElapsedTime = _elapsedTimePerFile?.Elapsed ?? TimeSpan.Zero;
            _progressInfo.CurrentFileSize = 0;
        }

        public ProgressInfo GetProgressInfo()
        {
            Debug.WriteLine($"[GetProgressInfo] Completion={_progressInfo.CompletionPercentage}%, " +
                            $"Files={_progressInfo.FilesCopied}/{_progressInfo.TotalFiles}, " +
                            $"Bytes={_progressInfo.BytesCopied}/{_progressInfo.TotalBytes}");

            return _progressInfo;
        }

        public void IncrementTotalBytes(long fileSize)
        {
            if (fileSize <= 0) return;
            Interlocked.Add(ref _totalBytes, fileSize);
            _progressInfo.TotalBytes = _totalBytes;
            _timeCalculator?.AddTotalBytes(fileSize);

            Debug.WriteLine($"[IncrementTotalBytes] Добавлено {fileSize}, Total={_totalBytes}");
        }
    }
}
