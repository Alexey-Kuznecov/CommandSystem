using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Decorators
{
    public class UndoableCommandDecorator : IUndoableCommand
    {
        private readonly ICommand _inner;
        private readonly Action<CommandContext>? _undoHandler;
        private readonly Func<CommandContext, bool>? _canUndoHandler;

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public UndoableCommandDecorator(
            ICommand inner,
            Action<CommandContext>? undoHandler = null,
            Func<CommandContext, bool>? canUndoHandler = null)
        {
            _inner = inner;
            _undoHandler = undoHandler;
            _canUndoHandler = canUndoHandler;
        }

        public bool CanExecute(CommandContext context) => _inner.CanExecute(context);
        public void Execute(CommandContext context) => _inner.Execute(context);

        public void Undo(CommandContext context) => _undoHandler?.Invoke(context);
        public bool CanUndo(CommandContext context) => _canUndoHandler?.Invoke(context) ?? false;
    }
}
