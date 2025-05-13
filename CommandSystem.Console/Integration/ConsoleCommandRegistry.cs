using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;
using CommandSystem.Infrastructure.Lifecycle;

namespace CommandSystem.Console.Integration
{

    public class ConsoleCommandRegistry : IConsoleCommandRegistry
    {
        private readonly Dictionary<string, IConsoleCommand> _commands = new(StringComparer.OrdinalIgnoreCase);

        public void Register(IConsoleCommand command)
        {
            if (_commands.ContainsKey(command.Name))
                throw new InvalidOperationException($"Command {command.Name} is already registered.");

            _commands[command.Name] = command;

            // Регистрация алиасов
            if (command.Aliases != null)
            {
                foreach (var alias in command.Aliases)
                {
                    _commands[alias] = command;
                }
            }
        }

        public IConsoleCommand? Find(string name)
        {
            _commands.TryGetValue(name, out var command);
            return command;
        }

        public IEnumerable<IConsoleCommand> GetAll() => _commands.Values;

        public IReadOnlyCollection<IConsoleCommand> GetAllCommands()
        {
            return _commands.Values.Distinct().ToList();
        }
    }
}
