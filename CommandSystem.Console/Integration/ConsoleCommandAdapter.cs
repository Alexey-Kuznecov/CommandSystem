using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration.CommandSystem.Console.Integration;
using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Console.Integration
{
    public class ConsoleCommandAdapter : IConsoleCommand
    {
        private readonly ConsoleCommandMetadata _metadata;

        public ConsoleCommandAdapter(ConsoleCommandMetadata metadata)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public string Name => _metadata.Name;
        public string Description => _metadata.Description ?? string.Empty;

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (_metadata.Handler == null)
                throw new InvalidOperationException($"No handler defined for command '{Name}'.");

            await _metadata.Handler(context, cancellationToken);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
