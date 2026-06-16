using CommandSystem.Abstractions;

namespace CommandSystem.Core.UndoRedo
{
    public interface IAsyncCommandWithResult<T>
    {
        Task<UndoToken?> ExecuteWithResultAsync(
            T parameter,
            CommandContext context,
            CancellationToken cancellationToken);
    }
}
