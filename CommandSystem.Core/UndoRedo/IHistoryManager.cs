using CommandSystem.Abstractions;

namespace CommandSystem.Core.UndoRedo
{
    public interface IHistoryManager
    {
        bool CanUndo { get; }
        bool CanRedo { get; }

        Task<UndoToken> UndoAsync();
        Task RedoAsync();
        void Push(UndoToken token);
    }
}