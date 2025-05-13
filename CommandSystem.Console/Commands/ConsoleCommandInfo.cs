using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Console.Commands
{
    public class ConsoleCommandInfo
    {
        public IEnumerable<string> Aliases { get; }
        public string CommandName { get; }
        public Type CommandType { get; }
        public string Description { get; }

        public ConsoleCommandInfo(string commandName, string description, Type commandType, IEnumerable<string> aliases)
        {
            CommandName = commandName;
            CommandType = commandType;
            Aliases = aliases;
            Description = description;
        }
    }
}
