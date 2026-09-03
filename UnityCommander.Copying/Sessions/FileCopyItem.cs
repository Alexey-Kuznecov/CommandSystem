
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnityCommander.Copying.Sessions
{
    public enum FileCopyStatus
    {
        Pending,       // Файл ожидает в очереди
        InProgress,    // Копирование идёт
        Paused,        // Копирование приостановлено
        Completed,     // Успешно завершено
        Failed,        // Ошибка при копировании
        Skipped,       // Пропущен (по фильтру, пользователем или из-за конфликта)
        Cancelled      // Отменён пользователем или системой
    }

    public class FileCopyItem : INotifyPropertyChanged
    {
        public string Source { get; }
        public string Destination { get; }

        private long _size;
        public long Size { get => _size; set { _size = value; OnPropertyChanged(); UpdateDisplayValues(); } }

        private long _bytesCopied;
        public long BytesCopied
        {
            get => _bytesCopied;
            set { _bytesCopied = value; OnPropertyChanged(); UpdateDisplayValues(); }
        }

        private int _progress;
        public int Progress { get => _progress; set { _progress = value; OnPropertyChanged(); } }

        private FileCopyStatus _status;
        public FileCopyStatus Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        private string _bytesCopiedText = string.Empty;
        public string BytesCopiedText { get => _bytesCopiedText; private set { _bytesCopiedText = value; OnPropertyChanged(); } }

        private string _fileSizeText = string.Empty;
        public string FileSizeText { get => _fileSizeText; private set { _fileSizeText = value; OnPropertyChanged(); } }

        public string FileName { get; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public FileCopyItem(string source, string destination, long size)
        {
            Source = source;
            Destination = destination;
            FileName = Path.GetFileName(source) ?? source;
            Size = size;
            Status = FileCopyStatus.Pending;
            BytesCopied = 0;
            UpdateDisplayValues();
        }

        public void UpdateDisplayValues()
        {
            Progress = Size > 0 ? (int)Math.Round((double)BytesCopied / Size * 100) : (BytesCopied > 0 ? 100 : 0);
            FileSizeText = Size >= 1024 ? $"{(double)Size / 1024 / 1024:F1} МБ" : $"{Size} байт";
            BytesCopiedText = BytesCopied >= 1024 ? $"{(double)BytesCopied / 1024 / 1024:F1} МБ" : $"{BytesCopied} байт";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
