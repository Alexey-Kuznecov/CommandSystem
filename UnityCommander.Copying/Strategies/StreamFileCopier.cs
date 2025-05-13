
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class StreamFileCopier : IFileCopier
    {
        public async Task CopyFileAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            const int bufferSize = 1024 * 16; // 16 KB
            var buffer = new byte[bufferSize];

            using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                int bytesRead;
                while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                }
            }
        }
    }
}
