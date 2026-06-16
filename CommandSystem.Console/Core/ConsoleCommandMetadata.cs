
using CommandSystem.Abstractions;

namespace CommandSystem.Console.Core
{
    public class ConsoleCommandMetadata
    {  
        public string Name => Metadata.Name;
        public string? Description => Metadata.Description;
        public CommandMetadata Metadata { get; }
        public IReadOnlyList<string> Aliases { get; }

        public Func<IConsoleCommandContext, CancellationToken, Task> Handler { get; }

        public ConsoleCommandMetadata(
            CommandMetadata metadata,
            Func<IConsoleCommandContext, CancellationToken, Task> handler,
            IEnumerable<string>? aliases = null)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Handler = handler;
            Aliases = aliases?.ToList() ?? new List<string>();
        }
    }
}
