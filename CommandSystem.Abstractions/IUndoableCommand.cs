
namespace CommandSystem.Abstractions
{
    public interface IUndoableCommand : ICommand
    {
        void Undo(CommandContext context);
        bool CanUndo(CommandContext context);
    }
}
