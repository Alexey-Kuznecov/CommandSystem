
using CommandSystem.Commands.Helper;
using CommandSystem.Commands.Performance;
using CommandSystem.Commands.UtilProcess;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using System.Diagnostics;
using System.Threading;
using UnityCommander.Native;

namespace CommandSystem.Commands
{
    [ConsoleCommand("processctl", "Управление процессами: приостановка, возобновление, завершение, информация.", "pctl")]
    public class ProcessControlCommand : IConsoleCommand
    {
        public string Name => "processctl";
        public string Description => "Управление процессами: suspend/resume/kill/info.";
        public IEnumerable<string> Aliases => ["pctl"];
        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var output = context.Output;
            var args = context.Arguments.Select(arg => ArgumentAliases.GetFullArgument(arg)).ToList();

            if (args.Count() < 2)
            {
                output.WriteError("Использование: processctl --action [suspend|resume|kill|info] processName");
                return;
            }

            if (args.First() != "--action")
            {
                output.WriteError("Необходимо указать аргумент (--action или -a).");
                return;
            }

            string action = args[1];
            string processName = args.Last();

            if (string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(processName))
            {
                output.WriteError("Необходимо указать действие (--action) и имя процесса.");
                return;
            }

            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                output.WriteWarning($"Процессы с именем \"{processName}\" не найдены.");
                return;
            }

            var successAction = false;

            try
            {
                await Task.Run(() =>
                {
                    switch (action.ToLower())
                    {
                        case "suspend":
                            foreach (var process in processes)
                                successAction =TaskManager.SuspendProcess(process.Id);
                            var procSuspend = successAction ? processes.Length : 0;
                            output.WriteSuccess($"Приостановлено {procSuspend} процесс(ов).");
                            break;

                        case "resume":
                            foreach (var process in processes)
                                successAction = TaskManager.ResumeProcess(process.Id);
                            var procResume = successAction ? processes.Length : 0;
                            output.WriteSuccess($"Возобновлено {procResume} процесс(ов).");
                            break;

                        case "kill":
                            foreach (var process in processes)
                                process.Kill();
                            output.WriteSuccess($"Завершено {processes.Length} процесс(ов).");
                            break;

                        case "info":
                            foreach (var process in processes)
                            {
                                output.WriteLine($"Процесс: {process.ProcessName}, PID: {process.Id}, Статус: {process.Responding}");
                            }
                            break;

                        default:
                            output.WriteError($"Неизвестное действие: {action}");
                            break;
                    }
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                output.WriteError($"Ошибка при выполнении действия: {ex.Message}");
            }
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
