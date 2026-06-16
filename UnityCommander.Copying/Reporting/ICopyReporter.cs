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
        //void OnSessionStarted(CopySession session);
        //void OnSessionPaused(CopySession session);
        //void OnSessionResumed(CopySession session);
        //void OnSessionCancelled(CopySession session);
        //void OnSessionCompleted(CopySession session);
        void PrepareFileList(IEnumerable<(string source, string destination, long size)> files);
        void OnFileStarted(CopySession session, string source, string destination, long size);
        void OnFileProgress(string source, long bytesCopied);
        void OnFileCompleted(string source, bool success);
        //void OnFileCategorized(CopySession session, string source, string category);
    }
}
