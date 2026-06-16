
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Reporting;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;

namespace Svetokop.Services
{
    public class CopyLogReporter : ICopyReporter
    {
        private readonly TimeSpan _minFileDuration = TimeSpan.FromSeconds(3); // минимальное время для логирования
        private readonly Dictionary<string, (long lastBytes, DateTime lastTime)> _fileSpeedData = new();

        private int _progressCounter;
        private long _bytesCopied;
        private DateTime _startTime;
        private readonly ObservableCollection<CopyLogEntry> _entries = new();
        private readonly ReadOnlyObservableCollection<CopyLogEntry> _readonlyEntries;

        public event Action? FilesChanged;

        public ReadOnlyObservableCollection<CopyLogEntry> Entries => _readonlyEntries;

        public CopyLogReporter()
        {
            _readonlyEntries = new ReadOnlyObservableCollection<CopyLogEntry>(_entries);
        }

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

            if (fileDuration.HasValue && fileDuration.Value < _minFileDuration)
                return;

            var entry = new CopyLogEntry
            {
                Type = type,
                Timestamp = DateTime.Now,
                Message = message,
                Metadata = ex
            };

            Application.Current?.Dispatcher.BeginInvoke(() => _entries.Add(entry));
        }

        public void Clear() => _entries.Clear();

        public void OnSessionStarted(CopySession session)
        {
            AddEntryInternal(session, CopyLogType.Info, $"{DateTime.Now} {Messages.SessionStarted} {session.SourcePath} -> {session.TargetPath}");
        }

        public void OnFileStarted(CopySession session, string filePath, string destination, long size) 
        {
            _fileSpeedData[filePath] = (0, DateTime.Now);
            _bytesCopied = 0;
            _startTime = DateTime.Now;
            AddEntryInternal(session, 
                CopyLogType.FileStarted,
                $"{DateTime.Now} {Messages.FileStarted} {filePath}",
                filePath, 
                verboseOnly: true);
        }

        public void OnFileProgress(CopySession session, string filePath, long bytesCopied, long totalBytes)
        {
            _bytesCopied += bytesCopied;
            _progressCounter++;
            if (_progressCounter % session.ProgressStep != 0)
                return;

            // Вычисляем скорость копирования
            var now = DateTime.Now;
            var last = _fileSpeedData[filePath];
            var deltaBytes = _bytesCopied - last.lastBytes;
            var deltaSeconds = (now - last.lastTime).TotalSeconds;
            var speed = deltaSeconds > 0 ? deltaBytes / deltaSeconds : 0; // байт/сек

            // Обновляем запись
            _fileSpeedData[filePath] = (_bytesCopied, now);

            // Переводим в MB/s
            var speedMb = speed / 1024d / 1024d;

            double copiedMb = _bytesCopied / 1024d / 1024d;
            double totalMb = totalBytes / 1024d / 1024d;
            double percent = totalBytes > 0 ? (_bytesCopied * 100.0 / totalBytes) : 0;

            AddEntryInternal(session, CopyLogType.FileProgress,
                $"🔄 File: {Path.GetFileName(filePath)} | {copiedMb:F2} MB / {totalMb:F2} MB ({percent:F1}%) | Speed: {speedMb:F2} MB/s",
                filePath,
                verboseOnly: true);
        }


        public void OnFileCompleted(CopySession session, string source, string destination, bool success)
        {
            //var finishTime = (endTime != default ? endTime : DateTime.Now);
            var elapsed = DateTime.Now - _startTime;

            // показываем миллисекунды для маленьких файлов
            string duration = elapsed.TotalSeconds < 1
                ? $"{elapsed.TotalMilliseconds:F0} ms"
                : elapsed.ToString(@"mm\:ss");

            string fileName = Path.GetFileName(source);

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

        public void OnFileCategorized(CopySession session, string source, string category)
        {
            //throw new NotImplementedException();
        }

        public void PrepareFileList(IEnumerable<(string source, string destination, long size)> files)
        {
            throw new NotImplementedException();
        }

        public void OnFileProgress(string source, long bytesCopied)
        {
            throw new NotImplementedException();
        }

        public void OnFileCompleted(string source, bool success)
        {
            throw new NotImplementedException();
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
