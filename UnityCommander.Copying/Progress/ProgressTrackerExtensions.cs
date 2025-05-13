
namespace UnityCommander.Copying.Progress
{
    public static class ProgressTrackerExtensions
    {
        public static void ReportFileCopied(this IProgressTracker tracker, long bytes, int files = 1)
        {
            tracker.UpdateProgress(bytes);
            for (int i = 0; i < files; i++)
                tracker.CompleteFile();
        }
    }
}
