using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
