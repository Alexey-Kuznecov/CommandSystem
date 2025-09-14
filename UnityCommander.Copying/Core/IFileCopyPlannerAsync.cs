using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Core
{
    public interface IFileCopyPlannerAsync
    {
        IAsyncEnumerable<DiscoveredItem> GetDiscoveredItemsAsyncEnumerable(string source, string target, CopyOptions options, CancellationToken token);
    }
}
