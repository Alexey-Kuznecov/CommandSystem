
using CommandSystem.Console.Core;

namespace CommandSystem.Console.Commands
{
    public abstract class ConsoleCommandBase
    {
        public abstract string Name { get; }
        public virtual IEnumerable<string> Aliases => Enumerable.Empty<string>();
        public abstract string Description { get; }

        public abstract Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default);
    }
}
