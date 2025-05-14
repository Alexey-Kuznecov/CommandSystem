
using AlexeyKuznetsov.Helper;
using CommandSystem.Commands.Performance;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using System.Diagnostics;
using System.Threading;

namespace CommandSystem.Commands
{
    [ConsoleCommand("test", "Команда для тестирования кода", "t")]
    public class TestCommand : IConsoleCommand
    {
        private IConsoleOutput _output;
        public string Name => "test";
        public string Description => "Команда для тестирования кода";
        public IEnumerable<string> Aliases => ["t"];

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var output = context.Output;
            string fullPath = @"E:\Projects\03._Tests\CopyFileTest\UnityCommander\obj\Debug\netcoreapp3.1\UnityCommander.Controls_wulwusfh_wpftmp.AssemblyInfo.cs";
            string anchor = @"E:\Projects\03._Tests\";

            string shortened = PathFormatter.ShortenForLog(fullPath, anchor);
            output.WriteLine(shortened); // => ..\..\Output\log.txt
        }
    }
}
