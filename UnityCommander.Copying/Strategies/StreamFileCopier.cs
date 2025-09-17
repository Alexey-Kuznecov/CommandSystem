
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Strategies
{
    public class StreamFileCopier : IFileCopier
    {
        public async Task CopyFileAsync(
            string sourcePath,
            string destinationPath,
            int bufferSize,
            Action<long> onBytesCopied,
            CancellationToken cancellationToken,
            Action waitIfPaused)
        {
            var buffer = new byte[bufferSize];

            using var sourceStream = new FileStream(
                sourcePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                FileOptions.Asynchronous);

            using var destinationStream = new FileStream(
                destinationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                FileOptions.Asynchronous);

            int bytesRead;
            while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                //waitIfPaused?.Invoke();
                cancellationToken.ThrowIfCancellationRequested();
                await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                onBytesCopied?.Invoke(bytesRead);
            }
        }
    }
}
