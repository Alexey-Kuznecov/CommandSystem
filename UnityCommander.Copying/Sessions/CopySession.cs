
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Sessions
{
    public interface ICopySession
    {
        CopyOptions Options { get; }
        bool IsRunning { get; }
        bool IsPaused { get; }
        bool IsCancelled { get; }

        long BytesCopied { get; }
        int FilesCopied { get; }
        int TotalFiles { get; }
        long TotalBytes { get; }

        IReadOnlyList<FileCopyErrorContext> Errors { get; }
        IReadOnlyList<FileCopySuccessContext> Successes { get; }

        void Start(long totalBytes, int totalFiles);
        void Pause();
        void Resume();
        void Cancel();
        void AddBytes(long bytes);
        void CompleteFile();
        void AddError(FileCopyErrorContext context);
        void AddSuccess(FileCopySuccessContext context);
    }
}
