
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using UnityCommander.Copying.Strategies;

namespace UnityCommander.Copying
{
    public class CopyExecutor : ICopyExecutor
    {
        public async Task ExecuteAsync(
            Func<CancellationToken, IAsyncEnumerable<DiscoveredItem>> provider,
            CopyContext context,
            CopyOptions options,
            CopySessionService sessionService)
        {
            ICopyExecutionStrategy strategy = options.UseParallel
               ? new ParallelExecutionStrategy()
               : new SequentialExecutionStrategy();

            // Собираем все элементы в список
            var items = new List<DiscoveredItem>();
            var asyncEnum = provider(sessionService.CancellationToken);
            await foreach (var item in asyncEnum.WithCancellation(sessionService.CancellationToken))
            {
                items.Add(item);
            }

            var files = items.OnlyFiles().ToList();

            long totalBytes = items.Sum(f => new FileInfo(f.Source).Length);
            sessionService.StartSession(totalBytes, files.Count);
            context.ProgressTracker.Start(totalBytes, files.Count);

            await strategy.ExecuteAsync(items, context, options, sessionService);
        }
    }
}
