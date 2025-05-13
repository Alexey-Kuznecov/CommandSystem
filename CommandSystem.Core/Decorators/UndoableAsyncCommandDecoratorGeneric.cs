using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Decorators
{
    public class UndoableAsyncCommandDecorator<T> : IUndoableAsyncCommand<T>
    {
        private readonly IAsyncCommand<T> _inner;
        private readonly Func<CommandContext, Task>? _undoHandler;
        private readonly Func<CommandContext, bool>? _canUndoHandler;

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public UndoableAsyncCommandDecorator(
            IAsyncCommand<T> inner,
            Func<CommandContext, Task>? undoHandler = null,
            Func<CommandContext, bool>? canUndoHandler = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _undoHandler = undoHandler;
            _canUndoHandler = canUndoHandler;
        }

        public bool CanExecute(T parameter, CommandContext context)
        {
            return _inner.CanExecute(parameter, context);
        }

        public Task ExecuteAsync(T parameter, CommandContext context, CancellationToken cancellationToken)
        {
            return _inner.ExecuteAsync(parameter, context, cancellationToken);
        }

        public bool CanUndo(CommandContext context)
        {
            return _canUndoHandler?.Invoke(context) ?? false;
        }

        public Task UndoAsync(CommandContext context)
        {
            if (_undoHandler is not null)
                return _undoHandler(context);

            return Task.CompletedTask;
        }
    }
}
