
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;

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

            var worker = new FileCopyWorker(context, sessionService, options);

            var tasks = items.Select(async item =>
            {
                await semaphore.WaitAsync(sessionService.CancellationToken);
                try
                {
                    await worker.CopyOneAsync(item);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
