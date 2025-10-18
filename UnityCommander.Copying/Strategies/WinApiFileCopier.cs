using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Sessions;

namespace UnityCommander.Copying.Strategies
{
    public class WinApiFileCopier : IFileCopier
    {
        public Task CopyFileAsync(string sourcePath, string destinationPath, int bufferSize, Action<long> onBytesCopied, CancellationToken cancellationToken, Action waitIfPaused)
        {
            throw new NotImplementedException();
        }

        public Task CopyFileAsync(string sourcePath, string destinationPath, int bufferSize, Action<long> onBytesCopied, CancellationToken cancellationToken, CopySessionService copySessionService)
        {
            throw new NotImplementedException();
        }
    }
}
