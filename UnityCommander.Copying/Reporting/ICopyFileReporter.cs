using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Sessions;

namespace UnityCommander.Copying.Reporting
{
    public interface ICopyFileReporter
    {
        ReadOnlyObservableCollection<FileCopyItem> Files { get; }
        void OnFileStarted(CopySessionService session, string source, string destination, long size, string category);
        void OnFileProgress(CopySessionService session, string source, long bytesCopied, long totalBytes);
        void OnFileCompleted(CopySessionService session, string source, string destination, bool success);
        void OnFileCategorized(CopySessionService session, string source, string category);
        void OnSessionCompleted(CopySessionService session);
    }
}
