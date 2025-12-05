
using CommandSystem.Console.Core;

namespace CommandSystem.Console.Commands
{
    public class DelegateConsoleCommand : IConsoleCommand
    {
        private Action<ConsoleCommandContext, IConsoleOutput> _handler;

        public string Name { get; }
        public string Description { get; }
        public IEnumerable<string> Aliases { get; }

        public DelegateConsoleCommand(
            string name,
            string description,
            Action<ConsoleCommandContext, IConsoleOutput> handler,
            IEnumerable<string>? aliases = null)
        {
            Name = name;
            Description = description;
            _handler = handler;
            Aliases = aliases ?? Enumerable.Empty<string>();
        }

        public void Execute(IConsoleCommandContext context)
        {
            if (context is not ConsoleCommandContext consoleContext)
                throw new InvalidOperationException("Context must be of type ConsoleCommandContext.");

            _handler(consoleContext, consoleContext.Output);
        }

        public IEnumerable<string> GetSuggestions(string[] args) => Enumerable.Empty<string>();

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
