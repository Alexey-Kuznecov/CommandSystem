using CommandSystem.Commands.IO;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;

namespace CommandSystem.Commands
{
    [ConsoleCommand("dirwatch", "Мониторит изменения в указанной директории", "dw", "watcher")]
    public class PluginDirectoryMonitorCommand : IConsoleCommand
    {
        private IConsoleOutput _output = new ConsoleOutput();
        private DirectoryWatcher _directoryWatcher;

        public string Name => "dirwatch";
        public string Description => "Мониторит изменения в указанной директории";
        public IEnumerable<string> Aliases => ["dw", "watcher"];

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            _output = context.Output;
            var directory = context?.Arguments.Length > 0 ? context.Arguments[0] : null; // предполагается, что путь к директории передан через аргументы

            if (directory == null)
            {
                _output.WriteError($"Укажите путь к папке для отслеживания изменений.");
                return;
            }

            if (!Directory.Exists(directory))
            {
                _output.WriteError($"Указаный путь не найден. {directory}");
                return;
            }
            
            // Создаём и начинаем мониторить директорию
            _directoryWatcher = new DirectoryWatcher(directory, _output);
            _output.WriteLine($"Начат мониторинг изменений в директории: {directory}");

            // Ожидаем сигнал об отмене
            try
            {
                // Ожидание с отменой задачи
                await Task.Delay(-1, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Задача была отменена, корректно завершаем выполнение
                _output.Clear();
                _output.WriteLine("Отслеживание завешено.");
            }
        }

        public void Dispose()
        {
            _directoryWatcher?.Dispose();
        }
    }
}
