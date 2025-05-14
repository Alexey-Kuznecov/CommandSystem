
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class MultiThreadedFileCopier //: IFileCopier
    {
        public async Task CopyFileAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            // Предположим, что для многопоточного копирования мы делим файл на несколько частей.
            const int chunkSize = 1024 * 1024; // 1 MB
            var fileInfo = new FileInfo(sourcePath);
            var totalChunks = (int)Math.Ceiling(fileInfo.Length / (double)chunkSize);

            for (int i = 0; i < totalChunks; i++)
            {
                var startByte = i * chunkSize;
                var length = Math.Min(chunkSize, (int)(fileInfo.Length - startByte));

                tasks.Add(Task.Run(async () =>
                {
                    using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                    using (var destinationStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buffer = new byte[length];
                        sourceStream.Seek(startByte, SeekOrigin.Begin);
                        await sourceStream.ReadAsync(buffer, 0, length, cancellationToken);
                        await destinationStream.WriteAsync(buffer, 0, length, cancellationToken);
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }
    }
}
