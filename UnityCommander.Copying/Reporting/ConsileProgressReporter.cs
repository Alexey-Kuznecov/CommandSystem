
using AlexeyKuznetsov.Helper;
using CommandSystem.Console.Core;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Reporting
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        private IConsoleOutput _consoleOutput;

        public ConsoleProgressReporter(IConsoleOutput output)
        {
            _consoleOutput = output;
        }

        public void Report(ProgressInfo progressInfo)
        {
            int progress = (int)Math.Round(progressInfo.CompletionPercentage);
            int barUnits = Math.Min(20, progress / 5);

            var progressBar = new string('#', barUnits);
            var remaining = new string('-', 20 - barUnits);

            var formattedBytes = ConverterBytes.AutoConvertFormatBytes(progressInfo.BytesCopied);
            _consoleOutput.Write($"\r[{progressBar}{remaining}] {progress,3}% ({progressInfo.FilesCopied}/{progressInfo.TotalFiles}), {formattedBytes.PadLeft(10)}");
        }

        private static string FormatSize(string sizeInMb)
        {
            return $"{sizeInMb:0.00} Mb".PadLeft(10);
        }
    }
}
