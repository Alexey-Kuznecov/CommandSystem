
namespace CommandSystem.Console.Commands
{
    public class ConsoleCommandParameter
    {
        public string Input { get; }
        public string[] Arguments { get; }

        public ConsoleCommandParameter(string input, string[] arguments)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }
    }
}
