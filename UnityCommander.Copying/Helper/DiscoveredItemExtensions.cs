using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Helper
{
    public static class DiscoveredItemExtensions
    {
        public static IEnumerable<DiscoveredItem> OnlyFiles(this IEnumerable<DiscoveredItem> items)
            => items.Where(i => i.Type == DiscoveredItemType.File);

        public static IEnumerable<DiscoveredItem> OnlyDirectories(this IEnumerable<DiscoveredItem> items)
            => items.Where(i => i.Type == DiscoveredItemType.Directory);
    }
}
