
using CommandSystem.Abstractions;

namespace CommandSystem.Core.UndoRedo
{
    public class CommandHistoryManager : IHistoryManager
    {
        private BatchUndoToken? _currentBatch;

        private readonly IHistoryStore _store;

        public CommandHistoryManager(IHistoryStore store)
        {
            _store = store;
        }

        public bool CanUndo => _store.CanUndo;
        public bool CanRedo => _store.CanRedo;

        public void Push(UndoToken token)
        {
            if (_currentBatch != null)
            {
                _currentBatch.Tokens.Add(token);
            }
            else
            {
                _store.PushUndo(token);
                _store.ClearRedo();
            }
        }

        public async Task<UndoToken?> UndoAsync()
        {
            var token = _store.PopUndo();
            if (token == null)
                return null;

            await token.UndoAsync();
            _store.PushRedo(token);

            return token;
        }

        public async Task RedoAsync()
        {
            var token = _store.PopRedo();
            if (token == null) return;

            if (token.RedoAsync != null)
                await token.RedoAsync();

            _store.PushUndo(token);
        }

        public void BeginBatch()
        {
            _currentBatch = new BatchUndoToken();
        }

        public void EndBatch()
        {
            if (_currentBatch != null)
            {
                _store.PushUndo(_currentBatch);
                _store.ClearRedo();
                _currentBatch = null;
            }
        }
    }
}
