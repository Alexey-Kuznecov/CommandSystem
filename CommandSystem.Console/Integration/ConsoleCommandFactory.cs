using CommandSystem.Console.Core;
using CommandSystem.Console.Integration.CommandSystem.Console.Integration;
using CommandSystem.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
