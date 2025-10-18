using AlexeyKuznetsov.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using static System.Collections.Specialized.BitVector32;

namespace UnityCommander.Copying.Strategies
{
    public class ParallelExecutionStrategy : ICopyExecutionStrategy
    {
        public async Task ExecuteAsync(
            IEnumerable<DiscoveredItem> items,
            CopyContext context,
            CopyOptions options,
            CopySessionService sessionService)
        {
            using var semaphore = new SemaphoreSlim(options.MaxConсurrentTasks);
            var tasks = items.Select(async item =>
            {
                await semaphore.WaitAsync(sessionService.CancellationToken);
                try { await FileCopyWorker.CopyOneAsync(item, context, options, sessionService); }
                finally { semaphore.Release(); }
            });
            await Task.WhenAll(tasks);
        }
    }
}
