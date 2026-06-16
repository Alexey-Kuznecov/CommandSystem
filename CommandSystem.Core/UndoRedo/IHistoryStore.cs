
using CommandSystem.Abstractions;

namespace CommandSystem.Core.UndoRedo
{
    public interface IHistoryStore
    {
        bool CanUndo { get; }
        bool CanRedo { get; }
        void PushUndo(UndoToken token);
        UndoToken? PopUndo();

        void PushRedo(UndoToken token);
        UndoToken? PopRedo();

        void ClearRedo();
    }
}
