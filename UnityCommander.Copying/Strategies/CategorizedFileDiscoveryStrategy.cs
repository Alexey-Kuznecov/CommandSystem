using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class CategorizedFileDiscoveryStrategy //: IFileDiscoveryStrategy
    {
        private readonly IFileCategorizer _categorizer;

        public CategorizedFileDiscoveryStrategy(IFileCategorizer categorizer)
        {
            _categorizer = categorizer ?? throw new ArgumentNullException(nameof(categorizer));
        }

        public IEnumerable<DiscoveredItem> Discover(string sourceRoot, string destinationRoot)
        {
            if (!Directory.Exists(sourceRoot))
                yield break;

            foreach (var file in Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(file);
                var category = _categorizer.CategorizeAsync(fileInfo).GetAwaiter().GetResult(); // синхронно для примера
                var destPath = Path.Combine(destinationRoot, category, fileInfo.Name);

                yield return new DiscoveredItem
                {
                    Source = file,
                    Destination = destPath,
                    FileSize = fileInfo.Length,
                    FileInfo = fileInfo,
                    Type = DiscoveredItemType.File,
                    HasFilesInside = false,
                    Category = category // если есть поле Category
                };
            }
        }
    }
}
