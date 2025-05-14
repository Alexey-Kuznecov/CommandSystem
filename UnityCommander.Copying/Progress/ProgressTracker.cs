
using System;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Progress
{
    public class ProgressTracker : IProgressTracker
    {
        private LogHelper FileLogger;

        private long _totalBytes;              // Общий объём данных для отслеживания
        private int _totalFiles;               // Общее количество файлов
        private long _bytesCopied;             // Сколько байт скопировано до текущего момента
        private int _filesCopied;              // Сколько файлов скопировано
        private ProgressInfo _progressInfo;    // Объект, содержащий информацию о прогрессе

        public ProgressTracker()
        {
            _progressInfo = new ProgressInfo();
            FileLogger = new LogHelper();
            FileLogger.FileLogger.LogInfo("ProgressTracker создан " + GetHashCode());
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
        }

        // Начало отслеживания прогресса для конкретного файла
        public void StartFile(string sourcePath, long size)
        {
            //FileLogger.FileLogger.LogInfo($"[StartFile] Начало копирования файла: {sourcePath}, размер = {size} байт");

            _progressInfo.CurrentFile = sourcePath;
            _progressInfo.TotalBytes = size;
            _progressInfo.BytesCopied = 0;
        }

        // Обновление прогресса (вызывается при копировании каждого байта)
        public void UpdateProgress(long bytesCopied)
        {
            //_bytesCopied += bytesCopied;
            Interlocked.Add(ref _bytesCopied, bytesCopied);
            _progressInfo.BytesCopied = _bytesCopied;

            // Прогресс по байтам
            double bytePart = _totalBytes > 0 ? (double)_bytesCopied / _totalBytes : 0.0;

            // Прогресс по количеству файлов
            double filePart = _totalFiles > 0 ? (double)_filesCopied / _totalFiles : 0.0;

            // Смешанный прогресс (вес: 80% байты, 20% файлы)
            double combinedProgress = (bytePart * 0.8 + filePart * 0.2) * 100.0;

            // Гарантируем максимум 100
            if (combinedProgress > 100.0)
                combinedProgress = 100.0;

            _progressInfo.CompletionPercentage = combinedProgress;

            _progressInfo.FilesCopied = _filesCopied;
            _progressInfo.TotalFiles = _totalFiles;

            // Если всё скопировано — фиксируем 100%
            if (_bytesCopied == _totalBytes && _filesCopied == _totalFiles)
            {
                _progressInfo.CompletionPercentage = 100.0;
            }
        }

        // Завершение отслеживания для конкретного файла
        public void CompleteFile()
        {
            Interlocked.Increment(ref _filesCopied);
            _progressInfo.FilesCopied = _filesCopied;
            //FileLogger.FileLogger.LogInfo($"[CompleteFile] Файл завершён. Скопировано файлов: {_filesCopied} / {_totalFiles}");

        }

        // Получение текущей информации о прогрессе
        public ProgressInfo GetProgressInfo()
        {
            //FileLogger.FileLogger.LogInfo($"[GetProgressInfo] Получение прогресса: {_progressInfo.CompletionPercentage:F2}%, файлы: {_filesCopied}/{_totalFiles}, байты: {_bytesCopied}/{_totalBytes}");

            return _progressInfo;
        }
    }
}
