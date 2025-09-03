using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem.Core.Metadata;
using CommandSystem.Infrastructure.Lifecycle;

namespace CommandSystem.Infrastructure.Execution
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandFactory _factory;
        private readonly ICommandExecutor _executor;
        private readonly ICommandRegistry _registry;

        public CommandDispatcher(ICommandFactory factory, ICommandExecutor executor, ICommandRegistry registry)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        // Регистрация команды
        public void Register(CommandMetadata metadata, Func<CommandContext, Task> executeAsync)
        {
            var command = _factory.CreateAsync(metadata, executeAsync);
            _registry.Register(new RegisteredCommand(metadata, command));
        }

        public void Register<T>(CommandMetadata metadata, Func<T, CommandContext, Task> executeAsync)
        {
            var command = _factory.CreateAsync(metadata, executeAsync);
            _registry.Register(new RegisteredCommand(metadata, command));
        }

        public void Register(CommandMetadata metadata, Action<CommandContext> execute)
        {
            var command = _factory.Create(metadata, execute);
            _registry.Register(new RegisteredCommand(metadata, command));
        }

        public async Task DispatchAsync(string commandName, CommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Get(commandName)?.Command;

            if (command == null)
                throw new InvalidOperationException($"Command '{commandName}' is not registered.");

            if (command is IAsyncCommand asyncCommand)
            {
                await _executor.ExecuteAsync(asyncCommand, context, cancellationToken);
            }
            else if (command is ICommand syncCommand)
            {
                _executor.Execute(syncCommand, context);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported command type: {command.GetType()}");
            }
        }

        // Выполнение команды без параметра
        public async Task DispatchAsync(CommandMetadata metadata, CommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Get(metadata.Name);

            if (command == null)
                throw new InvalidOperationException($"Command '{metadata.Name}' is not registered.");

            if (command is IAsyncCommand asyncCommand)
            {
                await _executor.ExecuteAsync(asyncCommand, context, cancellationToken);
            }
            else if (command is ICommand syncCommand)
            {
                _executor.Execute(syncCommand, context);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported command type: {command.GetType()}");
            }
        }

        // Выполнение команды с параметром
        public async Task DispatchAsync<T>(CommandMetadata metadata, T parameter, CommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Get(metadata.Name);

            if (command == null)
                throw new InvalidOperationException($"Command '{metadata.Name}' is not registered.");

            if (command is IAsyncCommand<T> typedCommand)
            {
                await _executor.ExecuteAsync(typedCommand, parameter, context, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"Command '{metadata.Name}' is not a parameterized async command.");
            }
        }

        // Отмена регистрации команды
        public void Unregister(string commandName)
        {
            _registry.Unregister(commandName);
        }

        // Полная очистка всех команд
        public void Clear()
        {
            _registry.UnregisterAll();
        }
    }
}
