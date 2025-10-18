using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Strategies
{
    public class DefaultFileCopierFactory : IFileCopierFactory
    {
        private readonly IFileCopier _winApiCopier = new WinApiFileCopier();
        private readonly IFileCopier _standardCopier = new StreamFileCopier();

        public IFileCopier CreateFor(DiscoveredItem item, CopyOptions options)
        {
            if (options.UseWinApi && item.FileSize > 64 * 1024)
                return _winApiCopier;
            return _standardCopier;
        }
    }
}
