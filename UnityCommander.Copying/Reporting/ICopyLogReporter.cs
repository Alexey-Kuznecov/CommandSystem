using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Sessions;

namespace UnityCommander.Copying.Reporting
{
    public interface ICopyLogReporter
    {
        public ReadOnlyObservableCollection<CopyLogEntry> Entries { get; }
        void OnSessionStarted(CopySessionService session);
        void OnFileStarted(CopySessionService session, string filePath);
        void OnFileProgress(CopySessionService session, string filePath, long bytesCopied, long totalBytes);
        void OnFileCompleted(CopySessionService session, string filePath, bool success);
        void OnSessionPaused(CopySessionService session);
        void OnSessionResumed(CopySessionService session);
        void OnSessionCancelled(CopySessionService session);
        void OnError(CopySessionService session, string filePath, Exception ex);
        void OnSessionCompleted(CopySessionService session);
    }
}
