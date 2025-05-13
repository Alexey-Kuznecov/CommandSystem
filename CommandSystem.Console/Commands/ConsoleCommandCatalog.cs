using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Console.Commands
{
    public class ConsoleCommandCatalog
    {
        private readonly List<ConsoleCommandInfo> _commands = new();

        public void AddCommand(string commandName, string description, Type commandType, IEnumerable<string> aliases)
        {
            _commands.Add(new ConsoleCommandInfo(commandName, description, commandType, aliases));
        }

        public IEnumerable<ConsoleCommandInfo> GetAllCommands()
        {
            return _commands;
        }

        public ConsoleCommandInfo? FindCommand(string name)
        {
            return _commands.FirstOrDefault(c => string.Equals(c.CommandName, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
