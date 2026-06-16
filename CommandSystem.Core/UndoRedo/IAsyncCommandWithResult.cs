using CommandSystem.Abstractions;

namespace CommandSystem.Core.UndoRedo
{
    public interface IAsyncCommandWithResult
    {
        Task<UndoToken?> ExecuteWithResultAsync(CommandContext context);
    }
}
