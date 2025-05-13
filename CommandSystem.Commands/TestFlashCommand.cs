
using CommandSystem.Commands.Performance;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;

namespace CommandSystem.Commands
{
    [ConsoleCommand("testflash", "Измерить скорость копирования файла определённого размера на заданный диск, повторив операцию несколько раз, и вывести среднюю скорость в MB/sec.", "tf")]
    public class TestFlashCommand : IConsoleCommand
    {
        public string Name => "testflash";

        public string Description => "Измеряет скорость копирования накопителя например Flash-Карту. Например: testflash drive=D: size=100 iter=10";
        public IEnumerable<string> Aliases => [ "tf" ];

        private readonly IFlashPerformanceTester _tester;

        public TestFlashCommand()
        {
            _tester = new FlashPerformanceTester();
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var output = context.Output;
            var args = context.Arguments;
            string drive = args.FirstOrDefault(a => a.StartsWith("drive="))?.Split('=')[1] ?? "С:";
            int size = int.TryParse(args.FirstOrDefault(a => a.StartsWith("size="))?.Split('=')[1], out var s) ? s : 200;
            int iterations = int.TryParse(args.FirstOrDefault(a => a.StartsWith("iter="))?.Split('=')[1], out var i) ? i : 5;

            output.WriteLine($"Тестирование скорости записи на {drive}: {iterations} итераций, {size}MB...");

            var result = _tester.TestAsync(drive, size * 1000000, iterations, cancellationToken);
            for (i = 0; i < result.Result.IterationSpeedsMbPerSec.Count; i++)
            {
                output.WriteLine($"Итерация {i + 1}: {result.Result.IterationSpeedsMbPerSec[i]} MB/s");
            }

             output.WriteLine($"Средняя скорость: {result.Result.AverageSpeedMbPerSec} MB/s");
        }
    }
}
