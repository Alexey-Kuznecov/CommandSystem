
using CommandSystem.Abstractions;

namespace CommandSystem.Console.Commands
{
    namespace CommandSystem.Console.Integration
    {
        public interface IConsoleCommandRegistry
        {
            void Register(string name, Action<CommandContext> execute);
            void Register<T>(string name, Func<T, CommandContext, Task> execute);
            void Register(string name, Func<CommandContext, Task> execute);
        }
    }
}
