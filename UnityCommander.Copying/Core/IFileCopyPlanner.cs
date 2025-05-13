using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Core
{
    public interface IFileCopyPlanner
    {
        //IEnumerable<(string Source, string Destination)> BuildPlan(IEnumerable<string> inputPaths, string destinationRoot, CopyOptions options);
        public Task<IEnumerable<(string Source, string Destination)>> GetFilesToCopyAsync(
            string sourceDirectory,
            string destinationDirectory,
            CopyOptions options,
            CancellationToken cancellationToken);
    }
}
