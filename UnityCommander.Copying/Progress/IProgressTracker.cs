
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Progress
{
    /// <summary>
    /// Интерфейс для отслеживания прогресса копирования или передачи данных.
    /// </summary>
    public interface IProgressTracker
    {
        /// <summary>
        /// Инициализирует отслеживание общей операции передачи.
        /// </summary>
        /// <param name="totalBytes">Общее количество байт для передачи.</param>
        /// <param name="totalFiles">Общее количество файлов для передачи.</param>
        void Start(long totalBytes, int totalFiles);

        /// <summary>
        /// Начинает отслеживание передачи отдельного файла.
        /// </summary>
        /// <param name="sourcePath">Путь к исходному файлу.</param>
        /// <param name="size">Размер файла в байтах.</param>
        void StartFile(string sourcePath, long size);

        /// <summary>
        /// Обновляет прогресс передачи.
        /// </summary>
        /// <param name="bytesCopied">Количество переданных байт с начала всей операции.</param>
        void UpdateProgress(long bytesCopied);

        /// <summary>
        /// Завершает отслеживание текущего файла.
        /// </summary>
        void CompleteFile();

        /// <summary>
        /// Возвращает текущую информацию о прогрессе.
        /// </summary>
        /// <returns>Объект <see cref="ProgressInfo"/>, содержащий данные о текущем состоянии операции.</returns>
        ProgressInfo GetProgressInfo();
        void IncrementTotalBytes(long fileSize);
    }
}
