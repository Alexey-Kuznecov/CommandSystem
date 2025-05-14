using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class DirectoryStructurePreservingStrategy // : IFileDiscoveryStrategy
    {
        public IEnumerable<DiscoveredItem> Discover(string sourcePath, string destinationRoot)
        {
            // Все директории (включая пустые)
            var directories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            foreach (var dir in directories)
            {
                var relativePath = Path.GetRelativePath(sourcePath, dir);
                var destination = Path.Combine(destinationRoot, relativePath);

                yield return new DiscoveredItem
                {
                    Source = dir,
                    Destination = destination,
                    Type = DiscoveredItemType.Directory
                };
            }

            // Все файлы
            var files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var relativePath = Path.GetRelativePath(sourcePath, file);
                var destination = Path.Combine(destinationRoot, relativePath);

                yield return new DiscoveredItem
                {
                    Source = file,
                    Destination = destination,
                    Type = DiscoveredItemType.File
                };
            }
        }
    }
}