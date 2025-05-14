using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Strategies
{
    public class FlatCopyStrategy //: IFileDiscoveryStrategy
    {
        public IEnumerable<(string Source, string Destination)> DiscoverFiles(string sourcePath, string destinationRoot)
        {
            var files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destination = Path.Combine(destinationRoot, fileName);
                yield return (file, destination);
            }
        }
    }
}
