using CommandSystem.Core.Commands;

namespace CommandSystem.Core.Abstractions;

public interface ICommandExecutor
{
    void Execute(ICommand command, CommandContext context);
    Task ExecuteAsync(IAsyncCommand command, CommandContext context, CancellationToken cancellationToken = default);
    Task ExecuteAsync<T>(IAsyncCommand<T> command, T parameter, CommandContext context, CancellationToken cancellationToken = default);
}