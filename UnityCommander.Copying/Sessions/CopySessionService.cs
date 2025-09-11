using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using static System.Collections.Specialized.BitVector32;

namespace CommandSystem.CopyTester.ViewModels
{
    public class CopySessionService //: ICopySessionService
    {
        private readonly ManualResetEventSlim _pauseEvent = new(true);
        private CancellationTokenSource? _cts;
        // Настройки копирования (маски, фильтры, атрибуты)
        public CopyOptions Options { get; set; } = new CopyOptions();
        public CancellationToken Token => _cts?.Token ?? CancellationToken.None;
        private readonly List<string> _copiedFiles = new();
        public IReadOnlyList<string> CopiedFiles => _copiedFiles;
        public required string SourcePath { get; set; }
        public required string TargetPath { get; set; }
        // Состояние текущей сессии
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }

        // Статистика
        public long BytesCopied { get; set; }
        public int FilesCopied { get; set; }
        public int TotalFiles { get; private set; }
        public long TotalBytes { get; private set; }

        // Списки ошибок и успешных файлов
        public List<FileCopyErrorContext> Errors { get; } = new();
        public List<FileCopySuccessContext> Successes { get; } = new();
        public CancellationToken CancellationToken { get; set; }

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
            _cts = new CancellationTokenSource();
            IsCancelled = false;
        }

        public void Pause() => _pauseEvent.Reset();  // блокируем
        public void Resume() => _pauseEvent.Set();   // разрешаем

        public void WaitIfPaused()
        {
            _pauseEvent.Wait(); // блокирует пока не Resume
            if (_cts?.Token.IsCancellationRequested ?? false)
                _cts.Token.ThrowIfCancellationRequested();
        }

        public void Cancel()
        {
            IsCancelled = true;
            _cts?.Cancel();
        }

        public void AddCopiedFile(string path)
        {
            lock (_copiedFiles)
            {
                _copiedFiles.Add(path);
            }
        }

        public void CleanupAfterCancel(IEnumerable<DiscoveredItem> plannedItems)
        {
            // Удаляем все файлы, которые могли быть скопированы
            foreach (var file in plannedItems.OnlyFiles())
            {
                try
                {
                    if (File.Exists(file.Destination))
                        File.Delete(file.Destination);
                }
                catch
                {
                    // Игнорируем ошибки удаления отдельных файлов
                }
            }

            // Удаляем папки в обратном порядке, чтобы сначала удалялись вложенные
            foreach (var dir in plannedItems.OnlyDirectories().OrderByDescending(d => d.Destination.Length))
            {
                try
                {
                    if (Directory.Exists(dir.Destination))
                        Directory.Delete(dir.Destination, false); // false — только если пустая
                }
                catch
                {
                    // Игнорируем ошибки удаления отдельных папок
                }
            }
        }

        // Методы обновления прогресса
        public void AddBytes(long bytes) => BytesCopied += bytes;
        public void CompleteFile() => FilesCopied++;
        public void AddError(FileCopyErrorContext context) => Errors.Add(context);
        public void AddSuccess(FileCopySuccessContext context) => Successes.Add(context);
    }
}
