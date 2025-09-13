using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Filtering
{
    internal class AllowAllFileFilter : IFileFilter
    {
        public bool ShouldCopy(string filePath)
        {
            return true;
        }
    }
}
