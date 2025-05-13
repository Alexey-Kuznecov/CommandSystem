using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Strategies
{
    public class RecursiveDirectoryPreservingStrategy : IFileDiscoveryStrategy
    {
        public IEnumerable<(string Source, string Destination)> DiscoverFiles(string sourcePath, string destinationRoot, CopyOptions options)
        {
            var files = Directory.GetFiles(sourcePath, "*", options.IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var relativePath = Path.GetRelativePath(sourcePath, file);
                var destination = Path.Combine(destinationRoot, relativePath);
                yield return (file, destination);
            }
        }
    }
}
