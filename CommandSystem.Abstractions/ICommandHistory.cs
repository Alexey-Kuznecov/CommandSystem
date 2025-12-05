
namespace CommandSystem.Abstractions
{
    public interface ICommandHistory
    {
        void Push(IUndoableCommand command, CommandContext context);
        bool CanUndo { get; }
        bool CanRedo { get; }

        void Undo();
        void Redo();
    }
}
