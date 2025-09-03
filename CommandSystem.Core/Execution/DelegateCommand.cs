using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;

namespace CommandSystem.Core.Execution
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<CommandContext> _execute;

        public string Name { get; }
        public string Description { get; }

        public DelegateCommand(string name, string description, Action<CommandContext> execute)
        {
            Name = name;
            Description = description;
            _execute = execute;
        }

        public bool CanExecute(CommandContext context) => true;

        public void Execute(CommandContext context) => _execute(context);
    }
}
