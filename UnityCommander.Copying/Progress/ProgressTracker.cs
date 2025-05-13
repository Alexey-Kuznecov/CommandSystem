
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Progress
{
    public class ProgressTracker : IProgressTracker
    {
        private long _totalBytes;              // Общий объём данных для отслеживания
        private int _totalFiles;               // Общее количество файлов
        private long _bytesCopied;             // Сколько байт скопировано до текущего момента
        private int _filesCopied;              // Сколько файлов скопировано
        private ProgressInfo _progressInfo;    // Объект, содержащий информацию о прогрессе

        public ProgressTracker()
        {
            _progressInfo = new ProgressInfo();
        }

        // Начало отслеживания общего прогресса
        public void Start(long totalBytes, int totalFiles)
        {
            _totalBytes = totalBytes;
            _totalFiles = totalFiles;
            _bytesCopied = 0;
            _filesCopied = 0;
            _progressInfo = new ProgressInfo();  // Обнуляем информацию о прогрессе
        }

        // Начало отслеживания прогресса для конкретного файла
        public void StartFile(string sourcePath, long size)
        {
            _progressInfo.CurrentFile = sourcePath;
            _progressInfo.TotalBytes = size;
            _progressInfo.BytesCopied = 0;
        }

        // Обновление прогресса (вызывается при копировании каждого байта)
        public void UpdateProgress(long bytesCopied)
        {
            _bytesCopied += bytesCopied;
            _progressInfo.BytesCopied = _bytesCopied;

            // Вычисляем CompletionPercentage
            if (_totalBytes > 0)
            {
                _progressInfo.CompletionPercentage = (double)_bytesCopied / _totalBytes * 100;
            }

            _progressInfo.FilesCopied = _filesCopied;
        }

        // Завершение отслеживания для конкретного файла
        public void CompleteFile()
        {
            _filesCopied++;
            _progressInfo.FilesCopied = _filesCopied;
        }

        // Получение текущей информации о прогрессе
        public ProgressInfo GetProgressInfo()
        {
            return _progressInfo;
        }
    }

}
