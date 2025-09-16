namespace UnityCommander.Copying.Reporting
{
    public class CopyLogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Категория/тип события (Info, Warning, Error, FileStarted и т.д.)
        /// </summary>
        public object? Type { get; set; }

        /// <summary>
        /// Пользовательское сообщение
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Полный путь исходного файла
        /// </summary>
        public string? SourcePath { get; init; }

        /// <summary>
        /// Полный путь целевого файла
        /// </summary>
        public string? TargetPath { get; init; }

        /// <summary>
        /// Размер файла (если известен)
        /// </summary>
        public long? FileSize { get; init; }

        /// <summary>
        /// Количество скопированных байт (для прогресса)
        /// </summary>
        public long? BytesCopied { get; init; }

        /// <summary>
        /// Время выполнения операции (например, для FileCompleted)
        /// </summary>
        public TimeSpan? Duration { get; init; }

        /// <summary>
        /// Id потока (ThreadId)
        /// </summary>
        public int? ThreadId { get; init; }

        /// <summary>
        /// Номер логического "копировщика" (CopyWorker #1 и т.п.)
        /// </summary>
        public int? WorkerId { get; init; }

        /// <summary>
        /// Дополнительные данные (например, Exception для ошибок)
        /// </summary>
        public object? Metadata { get; init; }

        public override string ToString()
        {
            // Удобный вывод для пользователя
            return $"[{Timestamp:HH:mm:ss}] {Type}: {Message}";
        }
    }
}