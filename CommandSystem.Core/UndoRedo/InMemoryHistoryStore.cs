
using CommandSystem.Abstractions;

namespace CommandSystem.Core.UndoRedo
{
    public class InMemoryHistoryStore : IHistoryStore
    {
        private readonly Stack<UndoToken> _undo = new();
        private readonly Stack<UndoToken> _redo = new();

        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;

        public void PushUndo(UndoToken token) => _undo.Push(token);

        public UndoToken? PopUndo()
            => _undo.Count > 0 ? _undo.Pop() : null;

        public void PushRedo(UndoToken token) => _redo.Push(token);

        public UndoToken? PopRedo()
            => _redo.Count > 0 ? _redo.Pop() : null;

        public void ClearRedo() => _redo.Clear();
    }
}
