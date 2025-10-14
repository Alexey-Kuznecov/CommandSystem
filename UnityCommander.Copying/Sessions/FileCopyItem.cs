using AlexeyKuznetsov.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

    public class FileCopyItem
    {
        public string Source { get; set; }

        public string Destination { get; set; }

        public long Size { get; set; }

        public long BytesCopied { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public FileCopyStatus Status { get; set; }

        public string FileName { get; set; }

        public string FileSizeText { get; set; } = string.Empty;


        public string BytesCopiedText { get; set; } = string.Empty;


        public string ProgressText { get; set; } = string.Empty;


        public int Progress { get; set; }

        public FileCopyItem(string source, string destination)
        {
            Source = source ?? string.Empty;
            Destination = destination ?? string.Empty;
            FileName = Path.GetFileName(source) ?? source;
            try
            {
                Size = new FileInfo(source).Length;
            }
            catch
            {
                Size = 0L;
            }

            Status = FileCopyStatus.Pending;
            BytesCopied = 0L;
            UpdateDisplayValues();
        }

        public void UpdateDisplayValues()
        {
            if (Size > 0)
            {
                Progress = (int)(BytesCopied * 100 / Size);
                Progress = Math.Clamp(Progress, 0, 100);
                ProgressText = $"{Progress}%";
            }
            else
            {
                Progress = ((BytesCopied > 0) ? 100 : 0);
                ProgressText = $"{Progress}%";
            }

            FileSizeText = ((Size >= 1024) ? $"{(double)Size / 1024.0 / 1024.0:F1} МБ" : $"{Size} байт");
            BytesCopiedText = ((BytesCopied >= 1024) ? $"{(double)BytesCopied / 1024.0 / 1024.0:F1} МБ" : $"{BytesCopied} байт");
        }
    }
}
