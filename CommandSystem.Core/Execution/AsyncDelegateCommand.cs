
using CommandSystem.Abstractions;

namespace CommandSystem.Core.Execution
{
    public class AsyncDelegateCommand : IAsyncCommand
    {
        private readonly Func<CommandContext, Task> _execute;
        private readonly Func<CommandContext, bool>? _canExecute;
        private bool _isExecuting;

        public string Name { get; }
        public string Description { get; }

        public AsyncDelegateCommand(string name, string description, Func<CommandContext, Task> execute, Func<CommandContext, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            Name = name;
            Description = description;
        }

        public bool IsExecuting => _isExecuting;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(CommandContext context)
        {
            return !_isExecuting && (_canExecute?.Invoke(context) ?? true);
        }

        // Реализуем метод без поддержки CancellationToken
        public Task ExecuteAsync(CommandContext context)
        {
            if (!CanExecute(context)) return Task.CompletedTask;

            return ExecuteAsyncWithCancellation(context, CancellationToken.None);
        }

        // Реализуем метод с поддержкой CancellationToken
        public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (!CanExecute(context)) return;

            await ExecuteAsyncWithCancellation(context, cancellationToken);
        }

        private async Task ExecuteAsyncWithCancellation(CommandContext context, CancellationToken cancellationToken)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _execute(context); // Если _execute поддерживает отмену, передавай cancellationToken
            }
            catch (OperationCanceledException)
            {
                // Логика при отмене выполнения
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void Execute(CommandContext context)
        {
            // Вызовем асинхронно, но "забыто" — т.е. fire-and-forget
            _ = ExecuteAsync(context); // Вызов с пустым cancellationToken, если его нет
        }

        private void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
