using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Core
{
    public interface IFileCopyPlanner
    {
        public Task<IEnumerable<DiscoveredItem>> GetDiscoveredItems(
            string sourceDirectory,
            string destinationDirectory,
            CopyOptions options,
            CancellationToken cancellationToken);
    }
}
