
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class AsyncFileCopier : IFileCopier
    {
        public async Task CopyFileAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken)
        {
            await Task.Run(() => new StreamFileCopier().CopyFileAsync(sourceFile, destinationFile, cancellationToken), cancellationToken);
        }
    }
}
