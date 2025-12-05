
using CommandSystem.Console.Core;

namespace CommandSystem.Console.Integration
{
    public class ConsoleCommandFactory
    {
        public IConsoleCommand Create(ConsoleCommandMetadata metadata)
        {
            return new ConsoleCommandAdapter(metadata);
        }
    }
}
