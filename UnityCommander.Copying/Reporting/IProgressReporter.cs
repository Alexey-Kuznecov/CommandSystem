
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Reporting
{
    public interface IProgressReporter
    {
        void Report(ProgressInfo info);
    }
}
