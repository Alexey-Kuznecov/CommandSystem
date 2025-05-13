using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Settings;
using UnityCommander.Copying.Strategies;

namespace UnityCommander.Copying.Strategies
{
    public interface IFileDiscoveryStrategy
    {
        IEnumerable<(string Source, string Destination)> DiscoverFiles(string sourcePath, string destinationRoot, CopyOptions options);
    }
}

