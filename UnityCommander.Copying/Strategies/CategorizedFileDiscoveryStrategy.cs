
using System.Runtime.CompilerServices;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class CategorizedFileDiscoveryStrategy : IFileDiscoveryStrategy
    {
        private readonly IFileCategorizer _categorizer;

        public CategorizedFileDiscoveryStrategy(IFileCategorizer categorizer)
        {
            _categorizer = categorizer ?? throw new ArgumentNullException(nameof(categorizer));
        }

        public async IAsyncEnumerable<DiscoveredItem> DiscoverAsync(
            string sourceRoot,
            string destinationRoot,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(sourceRoot))
                yield break;

            foreach (var file in Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileInfo = new FileInfo(file);
                var category = await _categorizer.CategorizeAsync(fileInfo).ConfigureAwait(false);
                var filePath = Path.Combine(destinationRoot, category ?? string.Empty, fileInfo.Name);
                var destPath = Path.GetDirectoryName(filePath) ?? destinationRoot;
                if (!Directory.Exists(destPath))
                    Directory.CreateDirectory(destPath);

                yield return new DiscoveredItem
                {
                    Source = file,
                    Destination = filePath,
                    FileSize = fileInfo.Length,
                    FileInfo = fileInfo,
                    Type = DiscoveredItemType.File,
                    HasFilesInside = false,
                    Category = category ?? string.Empty
                };
            }
        }
    }

}
