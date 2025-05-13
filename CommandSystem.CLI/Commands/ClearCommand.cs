using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.CLI.Commands
{
    public class ClearCommand : ConsoleCommandBase
    {
        public override string Name => "clear";
        public override IEnumerable<string> Aliases => new[] { "cls" };
        public override string Description => "Очищает консоль.";

        public override Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            System.Console.Clear(); // Explicitly use System.Console to avoid ambiguity
            return Task.CompletedTask;
        }
    }
}
