using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Sessions;

namespace UnityCommander.Copying.Reporting
{
    public class CompositeCopyReporter //: ICopyReporter
    {
        //private readonly List<ICopyReporter> _reporters = new();

        //public CompositeCopyReporter(IEnumerable<ICopyReporter> reporters)
        //{
        //    _reporters.AddRange(reporters);
        //}

        //public void OnSessionStarted(CopySession session) => _reporters.ForEach(r => r.OnSessionStarted(session));
        //public void OnSessionPaused(CopySession session) => _reporters.ForEach(r => r.OnSessionPaused(session));
        //public void OnSessionResumed(CopySession session) => _reporters.ForEach(r => r.OnSessionResumed(session));
        //public void OnSessionCancelled(CopySession session) => _reporters.ForEach(r => r.OnSessionCancelled(session));
        //public void OnSessionCompleted(CopySession session) => _reporters.ForEach(r => r.OnSessionCompleted(session));

        //public void OnFileStarted(CopySession session, string source, string destination, long size)
        //    => _reporters.ForEach(r => r.OnFileStarted(session, source, destination, size));

        //public void OnFileProgress(CopySession session, string source, long bytesCopied, long totalBytes)
        //    => _reporters.ForEach(r => r.OnFileProgress(session, source, bytesCopied, totalBytes));

        //public void OnFileCompleted(CopySession session, string source, string destination, bool success)
        //    => _reporters.ForEach(r => r.OnFileCompleted(session, source, destination, success));

        //public void OnFileCategorized(CopySession session, string source, string category)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
