
using CommandSystem.Commands.UtilProcess;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using System.Diagnostics;
using UnityCommander.Native;

namespace CommandSystem.Commands
{
    [ConsoleCommand("proc-openfiles", "Выводит список открытых файлов указанного процесса по имени.", "procof")]
    public class ProcessOpenFilesCommand : IConsoleCommand
    {
        private IConsoleOutput _output;
        public string Name => "proc-openfiles";

        public string Description => "Выводит список открытых файлов указанного процесса по имени.";
        public IEnumerable<string> Aliases => ["procof"];

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var args = context.Arguments;
            _output = context.Output;

            if (args.Length < 2 || args[0] != "--name")
            {
                _output.WriteLine("Использование: proc-openfiles --name <имя_процесса>");
                return;
            }

            string processName = args[1];

            try
            {
                _output.Clear();
                // Ожидание с отменой задачи
                await ProcessMonitorTest.StartAsync(processName, _output, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Задача была отменена, корректно завершаем выполнение
                _output.Clear();
                _output.WriteLine("Операция завешена.");
            }
        }

        public void ProcessTest(string processName, CancellationToken cancellationToken)
        {
            var process = Process.GetProcessesByName(processName).FirstOrDefault();

            if (process == null)
            {
                _output.WriteLine($"Процесс с именем '{processName}' не найден.");
                return;
            }

            _output.WriteLine($"Открытые файлы процесса: {processName} (PID: {process.Id})");
            List<FileSystemInfo> infos = new();

            try
            {
                using var enumerator = ProcessUtility.GetOpenFilesEnumerator(process.Id);
                while (enumerator.MoveNext())
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    infos.Add(enumerator.Current);
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Ошибка при получении открытых файлов: {ex.Message}");
                return;
            }

            var sorted = infos.OrderBy(f => f.FullName, StringComparer.OrdinalIgnoreCase);
            foreach (var file in sorted)
            {
                _output.WriteLine(file.FullName);
            }
        }
    }
}
