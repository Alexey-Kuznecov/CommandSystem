using AlexeyKuznetsov.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Sessions
{
    public enum FileCopyStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed
    }

    public class FileCopyItem : INotifyPropertyChanged
    {
        private int _progress;
        private string _bytesCopiedText = string.Empty;
        private string _fileSizeText = string.Empty;
        private FileCopyStatus _status;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string Source { get; set; }
        public string Destination { get; set; }
        public long Size { get; set; }
        public long BytesCopied { get; set; }

        public FileCopyItem(string source, string destination)
        {
            Source = source;
            Destination = destination;
            Size = new FileInfo(source).Length;
            Status = FileCopyStatus.Pending;
        }

        public string FileSizeText
        {
            get => _fileSizeText;
            set { _fileSizeText = value; OnPropertyChanged(); }
        }
        public string BytesCopiedText
        {
            get => _bytesCopiedText;
            set { _bytesCopiedText = value; OnPropertyChanged(); }
        }

        public int Progress
        {
            get => _progress;
            set { _progress = value; OnPropertyChanged(); }
        }

        public FileCopyStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }
    }
}
