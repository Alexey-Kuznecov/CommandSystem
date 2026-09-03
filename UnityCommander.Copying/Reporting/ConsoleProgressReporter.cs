
using CommandSystem.Console.Core;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Progress;

namespace UnityCommander.Copying.Reporting
{
    public class ConsoleProgressReporter(IConsoleOutput output) : IProgressReporter
    {
        public event Action<ProgressInfo> ProgressChanged = delegate { }; // Initialize with an empty delegate to avoid null

        private IConsoleOutput _consoleOutput = output;
        private HumanReadableTimeCalculator _humanCalculator = new HumanReadableTimeCalculator();

        public void Report(ProgressInfo progressInfo)
        {
            int progress = (int)Math.Round(progressInfo.CompletionPercentage);
            int barUnits = Math.Min(20, progress / 5);

            var progressBar = new string('#', barUnits);
            var remaining = new string('-', 20 - barUnits);

            //var formattedBytes = ConverterBytes.AutoConvertFormatBytes(progressInfo.BytesCopied);
            var formattedBytes = $"{progressInfo.BytesCopied / 1024 / 1024:F2} MB";
            var speedFormatted = $"{progressInfo.SpeedBytesPerSecond / 1024 / 1024:F2} MB/s";
            var readable = _humanCalculator.GetDisplayValue(progressInfo.EstimatedTimeRemaining, DateTime.UtcNow);
            _consoleOutput.Write($"\r[{progressBar}{remaining}] {progress,3}% ({progressInfo.FilesCopied}/{progressInfo.TotalFiles}), {formattedBytes.PadLeft(5)} Speed: {speedFormatted.PadLeft(5)} ETA: {readable:mm\\:ss}");
        }
    }
}
