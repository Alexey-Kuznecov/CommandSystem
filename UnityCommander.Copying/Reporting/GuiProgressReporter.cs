
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Reporting
{
    public class GuiProgressReporter : IProgressReporter
    {
        public event Action<ProgressInfo> ProgressChanged = delegate { };

        public void Report(ProgressInfo info)
        {
            ProgressChanged?.Invoke(info);
        }
    }
}
