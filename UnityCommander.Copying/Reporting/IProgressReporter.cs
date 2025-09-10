
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Reporting
{
    public interface IProgressReporter
    {
        public event Action<ProgressInfo> ProgressChanged;
        void Report(ProgressInfo info);
    }
}
