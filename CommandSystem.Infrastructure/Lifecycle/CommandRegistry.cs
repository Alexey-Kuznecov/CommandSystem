
namespace CommandSystem.Infrastructure.Lifecycle
{
    using CommandSystem.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityCommander.Logging.Contracts;
    using UnityCommander.Logging.Core;
    using UnityCommander.Logging.Infrastructure;

    public class CommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<string, IRegisteredCommand> _commands = new();

        private readonly ILogger? _logger;

        public CommandRegistry(LoggerCreator logger)
        {
            _logger = logger.For<CommandRegistry>(LogScope.Runtime);
        }

        public void Register(IRegisteredCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (_commands.ContainsKey(command.Name))
                throw new InvalidOperationException($"Command '{command.Name}' is already registered.");

            _commands[command.Name] = command;
        }

        public void Unregister(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException("Command name cannot be null or whitespace.", nameof(commandName));

            if (_commands.TryGetValue(commandName, out var command))
            {
                if (command.Command is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _commands.Remove(commandName);
            }
        }

        public void UnregisterAll()
        {
            foreach (var command in _commands.Values)
            {
                if (command.Command is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _commands.Clear();
        }

        public IRegisteredCommand? Get(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                _logger?.Warning("Command name cannot be null or whitespace.");
                return null;
            }
                
            _commands.TryGetValue(commandName, out var command);
            return command;
        }

        public IReadOnlyCollection<IRegisteredCommand> GetAll()
        {
            return _commands.Values.ToList();
        }
    }
}
