using CommandSystem.Commands.IO;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;

namespace CommandSystem.Commands
{
    [ConsoleCommand("fileunlock", "Разблокирует файл занятый процессом", "fun", "unlk")]
    public class FileUnlockCommand : IConsoleCommand
    {
        private IConsoleOutput _output = new ConsoleOutput();

        public string Name => "fileunlock";
        public string Description => "Разблокирует файл занятый процессом";
        public IEnumerable<string> Aliases => ["fun", "unlk"];

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            _output = context.Output;
            var args = context.Arguments;

            string filePath = args[0]; // предполагается, что путь к файлу передан через аргументы
            if (!File.Exists(filePath))
            {
                _output.WriteError($"Указаный путь к файлу не существует. {filePath}");
                return;
            }
            
            if (string.IsNullOrEmpty(filePath))
            {
                _output.WriteError("Не указан путь к файлу.");
                return;
            }

            try
            {
                var processes = await Task.Run(() => FileUnlocker.WhoIsLocking(filePath), cancellationToken);

                if (processes.Count > 0)
                {
                    _output.WriteLine("Процессы, блокирующие файл:");
                    foreach (var process in processes)
                    {
                        _output.WriteLine($"PID: {process.Id}, Name: {process.ProcessName}");
                    }
                }
                else
                {
                    _output.WriteSuccess("Файл не заблокирован процессами.");
                }
            }
            catch (Exception ex)
            {
                _output.WriteError($"Ошибка при получении информации: {ex.Message}");
            }
        }
        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
