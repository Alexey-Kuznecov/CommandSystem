
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Reporting;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using UnityCommander.SystemMetrics;

namespace Svetokop.Services
{
    public class CopyLogReporter : ICopyReporter
    {
        private readonly ObservableCollection<CopyLogEntry> _entries = new();
        public ReadOnlyObservableCollection<CopyLogEntry> Entries => new(_entries);
        private readonly TimeSpan _minFileDuration = TimeSpan.FromSeconds(3); // минимальное время для логирования
        private readonly Dictionary<string, (long lastBytes, DateTime lastTime)> _fileSpeedData = new();

        private int _progressCounter;
        private long _totalBytes;
        private void AddEntryInternal(
            CopySession session,
            CopyLogType type,
            string message,
            string? filePath = null,
            Exception? ex = null,
            bool verboseOnly = false,
            TimeSpan? fileDuration = null)
        {
            //if (verboseOnly && !session.VerboseLogging)
            //    return;

            // Фильтр по длительности файла
            if (fileDuration.HasValue && fileDuration.Value < _minFileDuration)
                return;

            var entry = new CopyLogEntry
            {
                Type = type,
                Timestamp = DateTime.Now, // момент записи в журнал
                Message = message,
                Metadata = ex
            };

            Application.Current?.Dispatcher.Invoke(() => _entries.Add(entry));
        }

        public void Clear() => _entries.Clear();

        public void OnSessionStarted(CopySession session)
        {
            AddEntryInternal(session, CopyLogType.Info, $"{DateTime.Now} {Messages.SessionStarted} {session.SourcePath} -> {session.TargetPath}");
        }

        public void OnFileStarted(CopySession session, string filePath, string destination, long size) 
        {
            _fileSpeedData[filePath] = (0, DateTime.Now);
            AddEntryInternal(session, 
                CopyLogType.FileStarted,
                $"{DateTime.Now} {Messages.FileStarted} {filePath}",
                filePath, 
                verboseOnly: true);
        }

        public void OnFileProgress(CopySession session, string filePath, long bytesCopied, long totalBytes)
        {
            _progressCounter++;
            //if (_progressCounter % session.ProgressStep != 0)
            //    return;

            var now = DateTime.Now;
            var last = _fileSpeedData[filePath];
            var deltaBytes = bytesCopied - last.lastBytes;
            var deltaSeconds = (now - last.lastTime).TotalSeconds;
            var speed = deltaSeconds > 0 ? deltaBytes / deltaSeconds : 0; // байт/сек

            // Обновляем запись
            _fileSpeedData[filePath] = (bytesCopied, now);

            // Переводим в MB/s
            var speedMb = speed / 1024d / 1024d;

            double copiedMb = bytesCopied / 1024d / 1024d;
            double totalMb = totalBytes / 1024d / 1024d;
            double percent = totalBytes > 0 ? (bytesCopied * 100.0 / totalBytes) : 0;

            //// Вычисляем скорость копирования в MB/s
            //double speedMbPerSec = elapsed.TotalSeconds > 0
            //    ? copiedMb / elapsed.TotalSeconds
            //    : 0;

            AddEntryInternal(session, CopyLogType.FileProgress,
                $"🔄 File: {Path.GetFileName(filePath)} | {copiedMb:F2} MB / {totalMb:F2} MB ({percent:F1}%) | Speed: {speedMb:F2} MB/s",
                filePath,
                verboseOnly: true);
        }

        public void OnFileCompleted(CopySession session, string filePath, DateTime startTime, DateTime endTime, bool success)
        {
            var finishTime = (endTime != default ? endTime : DateTime.Now);
            var elapsed = finishTime - startTime;

            // показываем миллисекунды для маленьких файлов
            string duration = elapsed.TotalSeconds < 1
                ? $"{elapsed.TotalMilliseconds:F0} ms"
                : elapsed.ToString(@"hh\:mm\:ss");

            string fileName = Path.GetFileName(filePath);

            if (success)
            {
                AddEntryInternal(session, CopyLogType.FileCompleted,
                    $"✅ Completed | File: {fileName} | Duration: {duration}");
            }
            else
            {
                AddEntryInternal(session, CopyLogType.FileCompleted,
                    $"⚠️ Failed | File: {fileName} | Duration: {duration}",
                    verboseOnly: true);
            }
        }

        public void OnSessionPaused(CopySession session)
        {
            var totalBytes = session.TotalBytes;
            var totalFiles = session.TotalFiles;
            var bytesCopied = session.BytesCopied;
            var filesCopied = session.FilesCopied;

            long remainingBytes = totalBytes - bytesCopied;
            int remainingFiles = totalFiles - filesCopied;

            AddEntryInternal(session, CopyLogType.Paused,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Сессия приостановлена | " +
                $"Файлы: {filesCopied}/{totalFiles} | " +
                $"Объём: {bytesCopied / 1024d / 1024d:F2} MB / {totalBytes / 1024d / 1024d:F2} MB | " +
                $"Осталось: {remainingFiles} файлов, {remainingBytes / 1024d / 1024d:F2} MB");
        }

        public void OnSessionResumed(CopySession session) =>
            AddEntryInternal(session, CopyLogType.Resumed,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Сессия возобновлена");

        public void OnSessionCancelled(CopySession session) =>
            AddEntryInternal(session, CopyLogType.Cancelled,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Сессия отменена");

        public void OnError(CopySession session, string filePath, Exception ex) =>
            AddEntryInternal(session, CopyLogType.Error,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Ошибка при копировании файла {Path.GetFileName(filePath)}",
                filePath, ex);

        public void OnSessionCompleted(CopySession session)
        {
            AddEntryInternal(session, CopyLogType.Info,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Сессия завершена | " +
                $"Скопировано файлов: {session.FilesCopied}/{session.TotalFiles}, " +
                $"Общий объём: {session.BytesCopied / 1024d / 1024d:F2} MB");
            _progressCounter = 0;
        }

        public void OnFileCompleted(CopySession session, string source, string destination, bool success)
        {
            //throw new NotImplementedException();
        }

        public void OnFileCategorized(CopySession session, string source, string category)
        {
            //throw new NotImplementedException();
        }

        private static class Messages
        {
            public static string SessionStarted     = $"ℹ️ Сессия запущена: ";
            public static string FileStarted        = $"📂 Начало копирования файла ";
            public static string FileProgress       = $"🔄 Файл в процессе копирования: ";
            public static string FileCompletedOk    = $"✅ Конец копирования файла ";
            public static string FileCompletedErr   = $"⚠️ Ошибка при копировании файла ";
            public static string SessionPaused      = $"⏸️ Сессия на паузе";
            public static string SessionResumed     = $"🔁 Сессия возобновлена";
            public static string SessionCancelled   = $"❌ Сессия отменена";
            public static string Error              = $"⚠️ Ошибка копирования";
            public static string SessionCompleted   = $"✅ Сессия завершена";
        }
    }
}
