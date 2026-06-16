using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Strategies
{
    public class SequentialExecutionStrategy : ICopyExecutionStrategy
    {
        public async Task ExecuteAsync(
            IEnumerable<DiscoveredItem> items,
            CopyContext context,
            CopyOptions options,
            CopySessionService sessionService)
        {
            var worker = new FileCopyWorker(context, sessionService, options);
            foreach (var item in items)
                await worker.CopyOneAsync(item);
        }
    }
}
