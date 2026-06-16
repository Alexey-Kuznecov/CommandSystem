
using CommandSystem.Abstractions;
using CommandSystem.Core.UndoRedo;

namespace CommandSystem.Core.Decorators
{
    public class UndoableAsyncCommandDecorator<T> : IAsyncCommand<T>
    {
        private readonly IAsyncCommand<T> _inner;
        private readonly IHistoryManager _history;

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public UndoableAsyncCommandDecorator(
            IAsyncCommand<T> inner,
            IHistoryManager history)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _history = history ?? throw new ArgumentNullException(nameof(history));
        }

        public bool CanExecute(T parameter, CommandContext context)
        {
            return _inner.CanExecute(parameter, context);
        }

        public async Task ExecuteAsync(T parameter, CommandContext context, CancellationToken cancellationToken)
        {
            // 💥 ВАЖНО: пытаемся получить UndoToken

            if (_inner is IAsyncCommandWithResult<T> withResult)
            {
                var token = await withResult.ExecuteWithResultAsync(parameter, context, cancellationToken);

                if (token != null)
                    _history.Push(token);
            }
            else
            {
                // обычная команда без undo
                await _inner.ExecuteAsync(parameter, context, cancellationToken);
            }
        }
    }
}
