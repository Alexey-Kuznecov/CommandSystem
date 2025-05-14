using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Reporting
{
    public class NullCopyMetricsCollector : ICopyMetricsCollector
    {
        public void OnFileCopyStarted(string s, string d) { }
        public void OnFileCopyCompleted(string s, string d, long b, TimeSpan t) { }
        public void OnError(string s, Exception ex) { }
        public void OnDirectoryCreated(string p) { }
        public void ReportFinal() { }
    }
}
