using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using CommandSystem.Core.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace CommandSystem.Commands
{
    [ConsoleCommand("exit", "Выходит из консоли.", "e", "ex")]
    public class ExitCommand : IConsoleCommand
    {
        private IServiceProvider _service;

        public string Name => "exit";
        public IEnumerable<string> Aliases => [ "e", "ex" ];
        public string Description => "Выходит из консоли.";

        public ExitCommand(IServiceProvider services)
        {
            _service = services;
        }

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
             var lifetime = _service.GetService<ConsoleApplicationLifetime>();
            if (lifetime != null)
            {
                lifetime.Stop();
            }
            return Task.CompletedTask;
        }
    }
}
