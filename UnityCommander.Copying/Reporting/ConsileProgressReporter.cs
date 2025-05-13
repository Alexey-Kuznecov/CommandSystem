
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
            var progress = (int)(progressInfo.CompletionPercentage);
            var progressBar = new string('#', progress / 5);
            var remaining = new string('-', 20 - progress / 5);
            _consoleOutput.Write($"\r[{progressBar}{remaining}] {progress}% ({progressInfo.FilesCopied}/{progressInfo.TotalFiles})");
        }
    }
}
