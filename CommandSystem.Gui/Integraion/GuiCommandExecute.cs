
using CommandSystem.Abstractions;

namespace CommandSystem.Gui.Integraion
{
    public class GuiCommandExecute
    {
        private readonly ICommandDispatcher _dispatcher;
        private readonly IServiceProvider _services;

        public GuiCommandExecute(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public CommandContext Execute(string commandName, object? parameter = null, CommandContext? context = null, CancellationToken cancellationToken = default)
        {
            var ctx = new CommandContext(commandName, _services, parameter, cancellationToken);
            _dispatcher.Dispatch(commandName, ctx, cancellationToken);
            return ctx;
        }

        public Task ExecuteAsync(string commandName, object? parameter = null, CommandContext? context = null, CancellationToken cancellationToken = default)
         {
            var ctx = new CommandContext(commandName, _services, parameter, cancellationToken);
            return _dispatcher.DispatchAsync(commandName, ctx, cancellationToken);
        }

        public Task ExecuteAsync<T>(CommandMetadata metadata, T parameter, CommandContext context, CancellationToken cancellationToken = default)
        {
            return _dispatcher.DispatchAsync(metadata, parameter, context, cancellationToken);
        }
    }
}
