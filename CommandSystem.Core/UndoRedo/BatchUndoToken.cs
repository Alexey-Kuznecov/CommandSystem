
using CommandSystem.Abstractions;

namespace CommandSystem.Core.UndoRedo
{
    public class BatchUndoToken : UndoToken
    {
        public List<UndoToken> Tokens { get; } = new();

        public async Task UndoAsync()
        {
            foreach (var t in Tokens.AsEnumerable().Reverse())
                await t.UndoAsync();
        }

        public async Task RedoAsync()
        {
            foreach (var t in Tokens)
                if (t.RedoAsync != null)
                    await t.RedoAsync();
        }
    }
}
