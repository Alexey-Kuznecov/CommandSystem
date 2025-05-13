using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration.CommandSystem.Console.Integration;

namespace CommandSystem.Console.Integration
{
    public class ConsoleCommandInvoker : IConsoleCommandInvoker
    {
        private readonly IConsoleCommandRegistry _registry;

        public ConsoleCommandInvoker(IConsoleCommandRegistry registry)
        {
            _registry = registry;
        }

        public async Task InvokeAsync(string commandName, IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Find(commandName);

            if (command == null)
            {
                throw new InvalidOperationException($"Command '{commandName}' not found.");
            }

            if (command is DelegateConsoleCommand delegateCommand)
            {
                //await delegateCommand.Handler(context, cancellationToken);
            }
            else
            {
                await command.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}
