
using CommandSystem.Abstractions;
using CommandSystem.Core.UndoRedo;
using CommandSystem.Infrastructure.Lifecycle;
using System.Diagnostics;

namespace CommandSystem.Infrastructure.Execution
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandFactory _factory;
        private readonly ICommandExecutor _executor;
        private readonly ICommandRegistry _registry;
        private readonly IHistoryManager _history;

        public CommandDispatcher(
            ICommandFactory factory,
            ICommandExecutor executor,
            ICommandRegistry registry,
            IHistoryManager history)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _history = history ?? throw new ArgumentNullException(nameof(history));

            CommandSystemBootstrap.RegisterSystemCommands(registry, history);
        }

        // 🔹 Регистрация
        public void Register(CommandMetadata metadata, Func<CommandContext, Task<UndoToken>> executeAsync)
        {
            var command = _factory.CreateAsync(metadata, executeAsync);
            _registry.Register(new RegisteredCommand(metadata, command));
        }

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

        // 🔹 SYNC dispatch
        public void Dispatch(string commandName, CommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Get(commandName)?.Command;

            if (command == null)
                throw new InvalidOperationException($"Command '{commandName}' is not registered.");

            if (command is ICommand syncCommand)
            {
                _executor.Execute(syncCommand, context);
                return;
            }

            throw new InvalidOperationException($"Unsupported command type: {command.GetType()}");
        }

        // 🔥 ASYNC dispatch (главный)
        public async Task DispatchAsync(string commandName, CommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Get(commandName)?.Command;

            if (command == null)
                throw new InvalidOperationException($"Command '{commandName}' is not registered.");

            await HandleAsyncCommand(command, context, cancellationToken);
        }

        // 🔹 без параметра
        public async Task DispatchAsync(CommandMetadata metadata, CommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Get(metadata.Name)?.Command;

            if (command == null)
                throw new InvalidOperationException($"Command '{metadata.Name}' is not registered.");

            await HandleAsyncCommand(command, context, cancellationToken);
        }

        // 🔹 с параметром
        public async Task DispatchAsync<T>(
            CommandMetadata metadata,
            T parameter,
            CommandContext context,
            CancellationToken cancellationToken = default)
        {
            var command = _registry.Get(metadata.Name)?.Command;

            if (command == null)
                throw new InvalidOperationException($"Command '{metadata.Name}' is not registered.");

            if (command is IAsyncCommand<T> typedCommand)
            {
                await _executor.ExecuteAsync(typedCommand, parameter, context, cancellationToken);
                return;
            }

            throw new InvalidOperationException($"Command '{metadata.Name}' is not a parameterized async command.");
        }

        public void Unregister(string commandName)
        {
            _registry.Unregister(commandName);
        }

        public IRegisteredCommand? Get(string commandName) => _registry.Get(commandName);

        public IReadOnlyCollection<IRegisteredCommand> GetAll() => _registry.GetAll();

        public void Clear()
        {
            _registry.UnregisterAll();
        }

        // 🔥 ВСПОМОГАТЕЛЬНЫЙ МЕТОД
        private async Task HandleAsyncCommand(object command, CommandContext context, CancellationToken ct)
        {
            if (command is IAsyncCommand asyncCommand)
            {
                var token = await asyncCommand.ExecuteAsync(context);

                Debug.WriteLine(token);

                if (!context.SuppressHistory && token != null)
                {
                    _history.Push(token);
                }

                return;
            }

            if (command is ICommand syncCommand)
            {
                _executor.Execute(syncCommand, context);
                return;
            }

            throw new InvalidOperationException($"Unsupported command type: {command.GetType()}");
        }
    }
}
