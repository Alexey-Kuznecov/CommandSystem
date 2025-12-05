
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;

namespace CommandSystem.Commands
{
    [ConsoleCommand("echo", "Выводит аргументы обратно.", "ec")]
    public class EchoCommand : IConsoleCommand
    {
        public string Name => "echo";
        public string Description => "Выводит аргументы обратно.";
        public IEnumerable<string> Aliases => [ "ec" ];

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            var args = context.Arguments;
            var output = args.Length > 0 ? string.Join(' ', args) : "Нет аргументов.";
            context.Output.Write(output);
            return Task.CompletedTask;
        }
        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
