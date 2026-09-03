
using CommandSystem.Abstractions;

namespace CommandSystem.Gui
{
    public sealed class AsyncCommandAdapter : System.Windows.Input.ICommand
    {
        private CommandContext _ctx;
        private readonly IAsyncCommand _command;
        private bool _isExecuting;

        public AsyncCommandAdapter(IAsyncCommand command)
        {
            _ctx = new CommandContext();
            _command = command;
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && _command.CanExecute(new CommandContext() { Parameter = parameter });
        }

        public async void Execute(object? parameter)
        {
            if (_isExecuting) return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                await _command.ExecuteAsync(new CommandContext() { Parameter = parameter });
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        private void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
