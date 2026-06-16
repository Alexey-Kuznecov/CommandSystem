
using CommandSystem.Abstractions;

namespace CommandSystem.Core.Decorators
{
    public class UndoableAsyncCommandDecorator //: IUndoableAsyncCommand
    {
        private readonly IAsyncCommand _inner;
        private readonly Func<CommandContext, Task>? _undoHandler;
        private readonly Func<CommandContext, bool>? _canUndoHandler;

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public UndoableAsyncCommandDecorator(
            IAsyncCommand inner,
            Func<CommandContext, Task>? undoHandler = null,
            Func<CommandContext, bool>? canUndoHandler = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _undoHandler = undoHandler;
            _canUndoHandler = canUndoHandler;
        }

        public bool CanExecute(CommandContext context) => _inner.CanExecute(context);

        public Task ExecuteAsync(CommandContext context) => _inner.ExecuteAsync(context);

        public Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken) => _inner.ExecuteAsync(context, cancellationToken);

        public bool CanUndo(CommandContext context) => _canUndoHandler?.Invoke(context) ?? false;

        public Task UndoAsync(CommandContext context)
        {
            if (_undoHandler is not null)
            {
                return _undoHandler(context);
            }
            return Task.CompletedTask;
        }
    }
}
