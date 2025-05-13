using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Core
{
    public interface ICopyManager
    {
        Task CopyFilesAsync(IEnumerable<string> inputPaths, string destinationRoot, CopyOptions options, CancellationToken cancellationToken);
    }
}
