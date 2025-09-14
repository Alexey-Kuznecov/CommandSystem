using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class RecursiveFullDiscoveryStrategyAsync : IFileDiscoveryStrategyAsync
    {
        public async IAsyncEnumerable<DiscoveredItem> DiscoverAsync(
            string sourceRoot,
            string destinationRoot,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(sourceRoot))
                yield break;

            var stack = new Stack<string>();
            stack.Push(sourceRoot);

            while (stack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentSourcePath = stack.Pop();
                var relativePath = Path.GetRelativePath(sourceRoot, currentSourcePath);
                var destPath = Path.Combine(destinationRoot, relativePath);

                bool hasFilesInside = Directory.EnumerateFiles(currentSourcePath).Any();

                yield return new DiscoveredItem
                {
                    Source = currentSourcePath,
                    Destination = destPath,
                    Type = DiscoveredItemType.Directory,
                    HasFilesInside = hasFilesInside
                };

                // Поддиректории
                foreach (var dir in Directory.GetDirectories(currentSourcePath))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    stack.Push(dir);
                }

                // Файлы
                foreach (var file in Directory.GetFiles(currentSourcePath))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var relPath = Path.GetRelativePath(sourceRoot, file);
                    var destFilePath = Path.Combine(destinationRoot, relPath);
                    var fileInfo = new FileInfo(file);

                    yield return new DiscoveredItem
                    {
                        Source = file,
                        Destination = destFilePath,
                        FileSize = fileInfo.Length,
                        FileInfo = fileInfo,
                        Type = DiscoveredItemType.File,
                        HasFilesInside = false
                    };

                    // имитация асинхронности при больших каталогах (чтобы не блокировать UI)
                    await Task.Yield();
                }
            }
        }
    }
}
