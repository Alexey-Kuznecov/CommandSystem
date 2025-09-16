
using System.Collections.ObjectModel;
using UnityCommander.Copying.Sessions;
using UnityCommander.SystemMetrics;

namespace UnityCommander.Copying.Reporting
{
    public interface ICopyLogReporter
    {
        public ReadOnlyObservableCollection<CopyLogEntry> Entries { get; }
        void OnSessionStarted(CopySessionService session);
        void OnFileStarted(CopySessionService session, string filePath, string destination, long size, string category);
        void OnFileProgress(CopySessionService session, string filePath, long bytesCopied, long totalBytes, TimeSpan elapsed);
        void OnFileCompleted(CopySessionService session, string filePath, DateTime startTime, DateTime endTime, bool success);
        void OnSessionPaused(CopySessionService session);
        void OnSessionResumed(CopySessionService session);
        void OnSessionCancelled(CopySessionService session);
        void OnError(CopySessionService session, string filePath, Exception ex);
        void OnSessionCompleted(CopySessionService session);
        //void OnSessionCompleted(CopySessionService session, FinalCopyReport report);
    }
}
