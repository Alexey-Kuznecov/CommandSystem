using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.SystemMetrics;

namespace UnityCommander.Copying.Reporting
{
    public class NullCopyMetricsCollector : ICopyMetricsCollector
    {
        public void PrepareAllFilesCopy(string s, string d, bool UseMetrics) { }
        public void OnFileCopyStarted(string s, string d) { }
        public void OnFileCopyCompleted(string s, string d, long b, TimeSpan t) { }
        public void OnError(string s, Exception ex) { }
        public void OnDirectoryCreated(string p) { }

        public void ReportFinal() 
        {
            Console.WriteLine("⚠️ NullCopyMetricsCollector.ReportFinal() вызван — метрики не активны.");
        }

        public FinalCopyReport StopAndCollectReport()
        {
           return null;
        }
    }
}
