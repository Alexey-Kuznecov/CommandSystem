using CommandSystem.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Console.Core
{
    public class ConsoleCommandMetadata
    {
        public CommandMetadata Metadata { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string Name { get; internal set; }
        public string? Description { get; internal set; }
        public Func<IConsoleCommandContext, CancellationToken, Task> Handler { get; }

        public ConsoleCommandMetadata(CommandMetadata metadata, 
            Func<IConsoleCommandContext, CancellationToken, Task> handler,
            IEnumerable<string>? aliases = null)
        {
            Name = metadata.Name;
            Description = metadata.Description;
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Handler = handler;
            Aliases = aliases?.ToList() ?? new List<string>();
        }
    }
}
