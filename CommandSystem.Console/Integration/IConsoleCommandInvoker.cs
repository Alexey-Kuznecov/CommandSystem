
using CommandSystem.Console.Core;

namespace CommandSystem.Console.Integration
{
    public interface IConsoleCommandInvoker
    {
        Task InvokeAsync(string commandName, IConsoleCommandContext context, CancellationToken cancellationToken = default);
    }
}
