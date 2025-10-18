
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Core
{
    public interface IFileCopierFactory
    {
        IFileCopier CreateFor(DiscoveredItem item, CopyOptions options);
    }
}
