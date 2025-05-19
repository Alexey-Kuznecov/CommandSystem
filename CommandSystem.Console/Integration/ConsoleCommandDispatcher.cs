using CommandSystem.Console.Core;
using CommandSystem.Console.Integration.CommandSystem.Console.Integration;
using CommandSystem.Core.Metadata;
using CommandSystem.Infrastructure.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Console.Integration
{
    public class ConsoleCommandDispatcher
    {
        private readonly IConsoleCommandRegistry _registry;
        private readonly IConsoleCommandInvoker _invoker;
        private readonly ConsoleCommandFactory _factory;

        public ConsoleCommandDispatcher(
            IConsoleCommandRegistry registry,
            IConsoleCommandInvoker invoker,
            ConsoleCommandFactory factory)
        {
            _registry = registry;
            _invoker = invoker;
            _factory = factory;
        }

        // Регистрирует команду
        public void RegisterCommand(ConsoleCommandMetadata metadata)
        {
            var command = _factory.Create(metadata);
            _registry.Register(command);
        }

        public void RegisterCommand(IConsoleCommand command)
        {
            _registry.Register(command);
        }

        // Выполняет команду
        public async Task ExecuteCommandAsync(string commandName, IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            var output = context.Output;
            try
            {
                var command = _registry.Find(commandName);

                if (command == null)
                {
                    throw new InvalidOperationException($"Command '{commandName}' not found.");
                }

                await _invoker.InvokeAsync(commandName, context, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                output.WriteLine("Команда отменена.");
            }
            catch (InvalidOperationException)
            {
                output.WriteError($"Команда '{commandName}' не найдена.");
            }
        }

        public void RegisterDelegateCommand(
            string name,
            string description,
            Func<IConsoleCommandContext, CancellationToken, Task> handler,
            IEnumerable<string>? aliases = null)
        {
            var metadata = new ConsoleCommandMetadata(new CommandMetadata(name, description), handler, aliases?.ToList());
            RegisterCommand(metadata);
        }

        public async Task FinalizeAllCommandsAsync()
        {
            foreach (var cmd in _registry.GetAllCommands())
            {
                if (cmd is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (cmd is IDisposableCommand disposable)
                {
                    disposable.Dispose();
                }
                else if (cmd is IConsoleCommand cmdWithFinalize)
                {
                    await cmdWithFinalize.FinalizeAsync();
                }
            }
        }

        public void FinalizeAllCommands()
        {
            foreach (var cmd in _registry.GetAllCommands())
            {
                if (cmd is IDisposableCommand disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
