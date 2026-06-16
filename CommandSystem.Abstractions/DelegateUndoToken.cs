
namespace CommandSystem.Abstractions
{
    public class DelegateUndoToken : UndoToken
    {
        private readonly Func<Task> _undo;
        private readonly Func<Task>? _redo;

        public DelegateUndoToken(Func<Task> undo, Func<Task>? redo = null)
        {
            _undo = undo;
            _redo = redo;
        }

        public override Task UndoAsync() => _undo();
        public override Task RedoAsync() => _redo?.Invoke() ?? Task.CompletedTask;
    }
}
