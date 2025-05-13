using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Commands.Performance
{
    public class FlashPerformanceTester : IFlashPerformanceTester
    {
        public async Task<FlashPerformanceResult> TestAsync(string driveLetter, int fileSizeBytes, int iterations, CancellationToken cancellationToken = default)
        {
            var result = new FlashPerformanceResult();
            var appendIterations = fileSizeBytes / 100000;
            string randomText = GenerateRandomString(100000);

            for (int j = 1; j <= iterations; j++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string tempFile = Path.Combine(Path.GetTempPath(), $"flash_test_{j}.tmp");
                await File.WriteAllTextAsync(tempFile, string.Concat(Enumerable.Repeat(randomText, appendIterations)), cancellationToken);

                var start = DateTime.Now;
                string targetFile = Path.Combine(driveLetter + "\\", $"flash_test_{j}.tmp");
                File.Copy(tempFile, targetFile, true);
                var duration = DateTime.Now - start;

                File.Delete(tempFile);
                File.Delete(targetFile);

                double speed = Math.Round((fileSizeBytes / 1000.0) / duration.TotalMilliseconds, 2); // MB/s
                result.IterationSpeedsMbPerSec.Add(speed);
            }

            return result;
        }

        private string GenerateRandomString(int size)
        {
            var builder = new StringBuilder(size);
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                builder.Append((char)random.Next('A', 'Z' + 1));
            }
            return builder.ToString();
        }
    }
}
