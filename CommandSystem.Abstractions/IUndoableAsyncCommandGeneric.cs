
namespace CommandSystem.Abstractions
{
    public interface IUndoableAsyncCommand<T> : IAsyncCommand<T>
    {
        Task UndoAsync(CommandContext context);
        bool CanUndo(CommandContext context);
    }
}
