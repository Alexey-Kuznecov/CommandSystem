using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Sessions;

namespace UnityCommander.Copying.Reporting
{
    public interface ICopyReporter
    {
        void OnSessionStarted(CopySession session);
        void OnSessionPaused(CopySession session);
        void OnSessionResumed(CopySession session);
        void OnSessionCancelled(CopySession session);
        void OnSessionCompleted(CopySession session);

        void OnFileStarted(CopySession session, string source, string destination, long size);
        void OnFileProgress(CopySession session, string source, long bytesCopied, long totalBytes);
        void OnFileCompleted(CopySession session, string source, string destination, bool success);
        void OnFileCategorized(CopySession session, string source, string category);
    }
}
