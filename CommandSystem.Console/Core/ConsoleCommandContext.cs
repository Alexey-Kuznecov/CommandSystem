
namespace CommandSystem.Console.Core
{
    public class ConsoleCommandContext : IConsoleCommandContext
    {
        public string[] Arguments { get; }
        public IServiceProvider Services { get; } // или IReadOnlyDictionary<string, object>
        public IConsoleOutput Output { get; }     // для вывода результата
        public string Input { get; } 

        // public IPlugin? SourcePlugin { get; }     // команда может знать кто её вызвал (опционально)

        public ConsoleCommandContext(IServiceProvider services, IConsoleOutput output, string[] args)//, IServiceProvider services, IConsoleOutput output, IPlugin? sourcePlugin = null)
        {
            Arguments = args;
            Services = services;
            Output = output;
            //SourcePlugin = sourcePlugin;
        }
    }
}
