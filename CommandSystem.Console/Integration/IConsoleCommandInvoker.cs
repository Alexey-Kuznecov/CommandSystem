using CommandSystem.Console.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Console.Integration
{
    namespace CommandSystem.Console.Integration
    {
        public interface IConsoleCommandInvoker
        {
            Task InvokeAsync(string commandName, IConsoleCommandContext context, CancellationToken cancellationToken = default);
        }
    }
}
