
using CommandSystem.Abstractions;
using CommandSystem.Gui.Core;

namespace CommandSystem.Gui
{
    public class CommandCatalog : ICommandCatalog
    {
        private readonly Dictionary<string, ICommand> _commands = new();

        public void RegisterCommand(string name, ICommand command, object owner)
        {
            _commands[name] = command;
        }

        public ICommand? GetCommand(string name) => _commands.TryGetValue(name, out var cmd) ? cmd : null;
    }
}
