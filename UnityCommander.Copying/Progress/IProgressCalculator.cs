
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Progress
{
    public interface IProgressCalculator
    {
        //public double Calculate(long totalBytes, long copiedBytes, int totalFiles, int copiedFiles);
        public double Calculate(long totalBytes, long copiedBytes);
    }
}
