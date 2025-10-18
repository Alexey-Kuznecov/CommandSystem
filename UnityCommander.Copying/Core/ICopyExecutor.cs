using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Core
{
public interface ICopyExecutor
{
    Task ExecuteAsync(
        Func<CancellationToken, IAsyncEnumerable<DiscoveredItem>> provider,
        CopyContext context,
        CopyOptions options,
        CopySessionService sessionService);
}
}
