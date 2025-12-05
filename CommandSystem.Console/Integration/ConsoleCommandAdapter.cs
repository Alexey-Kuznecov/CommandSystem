
using CommandSystem.Console.Core;

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
