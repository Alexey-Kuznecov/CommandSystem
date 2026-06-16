
using CommandSystem.Abstractions;
using CommandSystem.Core.UndoRedo;

namespace CommandSystem.Core.Decorators
{
    //public class UndoableCommandDecorator : IUndoableCommand
    //{
    //    private readonly ICommand _inner;
    //    private readonly IHistoryManager _history;

    //    public string Name => _inner.Name;
    //    public string Description => _inner.Description;

    //    public UndoableCommandDecorator(
    //        ICommand inner,
    //        IHistoryManager history)
    //    {
    //        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    //        _history = history ?? throw new ArgumentNullException(nameof(history));
    //    }

    //    public bool CanExecute(CommandContext context)
    //        => _inner.CanExecute(context);

    //    public void Execute(CommandContext context)
    //    {
    //        // ✅ новая логика
    //        if (_inner is IUndoableCommandResult undoable)
    //        {
    //            var token = undoable.ExecuteWithUndo(context);

    //            if (token != null)
    //                _history.Push(token);
    //        }
    //        else
    //        {
    //            _inner.Execute(context);
    //        }
    //    }

    //    // ⚠️ оставляем для совместимости (но больше не используем)
    //    public void Undo(CommandContext context)
    //    {
    //        // теперь undo централизован
    //    }

    //    public bool CanUndo(CommandContext context) => false;
    //}
}
