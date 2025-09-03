using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
