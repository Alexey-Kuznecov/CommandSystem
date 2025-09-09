using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Settings;

namespace CommandSystem.CopyTester.ViewModels
{
    public class CopySessionService
    {
        // Настройки копирования (маски, фильтры, атрибуты)
        public CopyOptions Options { get; set; } = new CopyOptions();

        // Состояние текущей сессии
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }

        // Статистика
        public long BytesCopied { get; private set; }
        public int FilesCopied { get; private set; }
        public int TotalFiles { get; private set; }
        public long TotalBytes { get; private set; }

        // Списки ошибок и успешных файлов
        public List<FileCopyErrorContext> Errors { get; } = new();
        public List<FileCopySuccessContext> Successes { get; } = new();

        // Методы управления сессией
        public void StartSession(long totalBytes, int totalFiles)
        {
            TotalBytes = totalBytes;
            TotalFiles = totalFiles;
            BytesCopied = 0;
            FilesCopied = 0;
            Errors.Clear();
            Successes.Clear();
            IsRunning = true;
            IsPaused = false;
            IsCancelled = false;
        }

        public void Pause() => IsPaused = true;
        public void Resume() => IsPaused = false;
        public void Cancel() => IsCancelled = true;

        // Методы обновления прогресса
        public void AddBytes(long bytes) => BytesCopied += bytes;
        public void CompleteFile() => FilesCopied++;
        public void AddError(FileCopyErrorContext context) => Errors.Add(context);
        public void AddSuccess(FileCopySuccessContext context) => Successes.Add(context);
    }
}
