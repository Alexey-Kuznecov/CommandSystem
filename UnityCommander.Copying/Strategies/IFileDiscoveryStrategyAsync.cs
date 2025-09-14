using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public interface IFileDiscoveryStrategyAsync
    {
        IAsyncEnumerable<DiscoveredItem> DiscoverAsync(string sourceRoot, string destinationRoot, CancellationToken cancellationToken);
    }
}
