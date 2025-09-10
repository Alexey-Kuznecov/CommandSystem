using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;

namespace CommandSystem.Core.Abstractions;

public interface ICommandDispatcher
{
    void Dispatch(string commandName, CommandContext context, CancellationToken cancellationToken = default);
    Task DispatchAsync(CommandMetadata metadata, CommandContext context, CancellationToken cancellationToken = default);
    Task DispatchAsync(string commandName, CommandContext context, CancellationToken cancellationToken = default);
    Task DispatchAsync<T>(CommandMetadata metadata, T parameter, CommandContext context, CancellationToken cancellationToken = default);
}