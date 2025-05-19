
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class StreamFileCopier : IFileCopier
    {
        public async Task CopyFileAsync(string sourcePath, string destinationPath, Action<long> onBytesCopied, CancellationToken cancellationToken)
        {
            const int bufferSize = 1024 * 64; // увеличим буфер, 64 KB — хорошее значение
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
                cancellationToken.ThrowIfCancellationRequested();
                await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                onBytesCopied?.Invoke(bytesRead); // ← обновляется прогресс
            }
        }
    }
}
