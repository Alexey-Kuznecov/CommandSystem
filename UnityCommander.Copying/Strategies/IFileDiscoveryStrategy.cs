using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Strategies
{
    public interface IFileDiscoveryStrategy
    {
        IEnumerable<DiscoveredItem> Discover(string sourceRoot, string destinationRoot);
    }
}

