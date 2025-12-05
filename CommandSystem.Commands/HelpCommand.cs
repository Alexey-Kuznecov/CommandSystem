using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;

namespace CommandSystem.Commands
{
    [ConsoleCommand("help", "Показывает список всех доступных команд с их описаниями.","?", "h")]
    public class HelpCommand : IConsoleCommand
    {
        private ConsoleCommandCatalog _catalog;
        private IServiceProvider _service;

        public string Name => "help";
        public string Description => "Показывает все доступные команды и их описания.";
        public IEnumerable<string> Aliases => ["?", "h"];

        public HelpCommand(IServiceProvider service, ConsoleCommandCatalog catalog)
        {
            _catalog = catalog;
            _service = service;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            if (_catalog == null)
            {
                context?.Output.WriteLine("Службы не инициализированы.");
                return;
            }
            var commands = _catalog.GetAllCommands()
                .OrderBy(c => c.CommandName)
                .ToList();

            if (commands.Count == 0)
            {
                context?.Output.WriteLine("Нет зарегистрированных команд.");
                return;
            }

            context?.Output.WriteLine("Доступные команды:\n");

            foreach (var command in commands)
            {
                var aliases = (command?.Aliases != null && command.Aliases.Any())
                    ? $" (алиасы: {string.Join(", ", command.Aliases)})"
                    : "";

                context?.Output.WriteLine($"- {command?.CommandName}{aliases}:");
                context?.Output.WriteLine($"  {command?.Description}\n");
            }

            await Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
