
using System;
using System.Diagnostics;
using UnityCommander.Copying.Core;
using UnityCommander.SystemMetrics;

namespace UnityCommander.Copying.Progress
{
    public class ProgressTracker : IProgressTracker
    {
        private readonly IProgressCalculator _progressCalculator;

        private long _totalBytes;              // Общий объём данных для отслеживания
        private int _totalFiles;               // Общее количество файлов
        private long _bytesCopied;             // Сколько байт скопировано до текущего момента
        private int _filesCopied;              // Сколько файлов скопировано
        private ProgressInfo _progressInfo;    // Объект, содержащий информацию о прогрессе
        private readonly ISpeedCalculator? _speedCalculator;
        private Stopwatch? _elapsedTimePerFile; // Время, затраченное на копирование файла
        private EstimatedTimeCalculator _timeCalculator;

        public ProgressTracker(IProgressCalculator progressCalculator, ISpeedCalculator? speedCalculator = null)
        {
            _progressCalculator = progressCalculator ?? new ProgressCalculator();
            _speedCalculator = speedCalculator;
            _progressInfo = new ProgressInfo();
        }

        // Начало отслеживания общего прогресса
        public void Start(long totalBytes, int totalFiles)
        {
            //FileLogger.FileLogger.LogInfo($"[Start] Начало отслеживания: всего байт = {totalBytes}, всего файлов = {totalFiles}");

            _totalBytes = totalBytes; // 155 069 628 байт
            _totalFiles = totalFiles;
            _bytesCopied = 0;
            _filesCopied = 0;
            _progressInfo = new ProgressInfo();  // Обнуляем информацию о прогрессе
            _progressInfo.TotalFiles = totalFiles;  // Устанавливаем общее количество файлов
            _progressInfo.TotalBytes = totalBytes; // Устанавливаем общий объём байт
            _speedCalculator?.Reset();
            _timeCalculator = new EstimatedTimeCalculator(totalBytes);
        }

        // Начало отслеживания прогресса для конкретного файла
        public void StartFile(string sourcePath, long size)
        {
            _elapsedTimePerFile = Stopwatch.StartNew();
            _progressInfo.CurrentFile = sourcePath;
        }

        // Обновление прогресса (вызывается при копировании каждого байта)
        public void UpdateProgress(long bytesCopied)
        {
            long currentBytes = Interlocked.Add(ref _bytesCopied, bytesCopied);
            _progressInfo.BytesCopied = _bytesCopied;

            // === Скорость ===
            if (_speedCalculator != null)
            {
                _speedCalculator.Update(currentBytes);
                _progressInfo.SpeedBytesPerSecond = _speedCalculator.GetSpeedBytesPerSecond();
            }

            // === Подсчет времени ===
            _timeCalculator?.Update(_bytesCopied);
            var remaining = _timeCalculator?.GetEstimatedRemainingTime();
            if (remaining.HasValue)
                _progressInfo.EstimatedTimeRemaining = remaining.Value;

            // === Прогресс ===
            _progressInfo.CompletionPercentage = _progressCalculator.Calculate(_totalBytes, _bytesCopied);
           
            _progressInfo.FilesCopied = _filesCopied;
            _progressInfo.TotalFiles = _totalFiles;

            // Если всё скопировано — фиксируем 100%
            //if (_bytesCopied == _totalBytes && _filesCopied == _totalFiles)
            //{
            //    _progressInfo.CompletionPercentage = 100.0;
            //}
        }

        // Завершение отслеживания для конкретного файла
        public void CompleteFile()
        {
            _elapsedTimePerFile?.Stop();
            Interlocked.Increment(ref _filesCopied);
            _progressInfo.FilesCopied = _filesCopied;
            _progressInfo.ElapsedTime = _elapsedTimePerFile?.Elapsed ?? TimeSpan.Zero;
        }

        // Получение текущей информации о прогрессе
        public ProgressInfo GetProgressInfo()
        {
            return _progressInfo;
        }
    }
}
