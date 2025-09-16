using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.SystemMetrics;

namespace UnityCommander.Copying.Reporting
{
    public interface ICopyMetricsCollector
    {
        void PrepareAllFilesCopy(string source, string destination, bool UseMetrics);
        void OnFileCopyStarted(string source, string destination);
        void OnFileCopyCompleted(string source, string destination, long sizeBytes, TimeSpan duration);
        void OnError(string source, Exception ex);
        void OnDirectoryCreated(string path);
        FinalCopyReport StopAndCollectReport();
        void ReportFinal();
    }
}
