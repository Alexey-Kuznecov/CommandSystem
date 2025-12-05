
namespace CommandSystem.Abstractions
{
    public interface IUndoableAsyncCommand : IAsyncCommand
    {
        Task UndoAsync(CommandContext context);
    }
}
