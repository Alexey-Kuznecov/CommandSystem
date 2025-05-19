
using CommandSystem.Commands.Performance;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using System.Diagnostics;
using System.Threading;

namespace CommandSystem.Commands
{
    [ConsoleCommand("sysstat", "Выводит статистику производительности системы.", "sst")]
    public class SysStatCommand : IConsoleCommand
    {
        private IConsoleOutput _output;
        public string Name => "sysstat";
        public string Description => "Выводит статистику производительности системы.";
        public IEnumerable<string> Aliases => ["sst"];

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            _output = context.Output;
            var args = context.Arguments;

            //string disk = args.FirstOrDefault(a => a.StartsWith("--disk="))?.Split('=')[1] ?? "_Total";
            int intervalSeconds = int.TryParse(args.FirstOrDefault(a => a.StartsWith("--interval="))?.Split('=')[1], out var i) ? i : 1;

            string diskLetterArg = args.FirstOrDefault(a => a.StartsWith("--disk="))?.Split('=')[1]?.ToUpper() ?? "_Total";
            string resolvedDiskInstance = "_Total";

            if (diskLetterArg != "_Total")
                resolvedDiskInstance = ResolvePhysicalDiskInstance(diskLetterArg);

            using var monitor = new SystemPerformanceMonitor(resolvedDiskInstance);
            _output.Clear();

            while (!cancellationToken.IsCancellationRequested)
            {
                MonitorLoop(monitor);
                await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
            }
           
            _output.Clear();
            _output.WriteLine("Опереция была отменина");
        }

        public void MonitorLoop(SystemPerformanceMonitor monitor)
        {
            var stats = monitor.GetStats();

            string[] lines = stats.ToString().Split('\n');
            int top = _output.CursorTop;

            _output.SetCursorPosition(0, top);
            foreach (var line in lines)
            {
                _output.Write(line.TrimEnd().PadRight(_output.WindowWidth));
            }

            Thread.Sleep(1000);
            _output.SetCursorPosition(0, top);
        }

        private string ResolvePhysicalDiskInstance(string letter)
        {
            var category = new PerformanceCounterCategory("PhysicalDisk");
            string[] instanceNames = category.GetInstanceNames();

            return instanceNames.FirstOrDefault(name => name.EndsWith($"{letter.ToUpper()}:")) ?? "_Total";
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
