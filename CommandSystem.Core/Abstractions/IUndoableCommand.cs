using CommandSystem.Core.Commands;

namespace CommandSystem.Core.Abstractions
{
    public interface IUndoableCommand : ICommand
    {
        void Undo(CommandContext context);
        bool CanUndo(CommandContext context);
    }
}
