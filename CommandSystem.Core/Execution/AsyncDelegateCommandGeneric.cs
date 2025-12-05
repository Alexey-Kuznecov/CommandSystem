
using CommandSystem.Abstractions;

namespace CommandSystem.Core.Execution
{
    public class AsyncDelegateCommand<T> : IAsyncCommand<T>
    {
        private readonly Func<T, CommandContext, Task> _execute;
        private readonly Func<T, CommandContext, bool>? _canExecute;

        public string Name { get; }
        public string Description { get; }

        public AsyncDelegateCommand(
            string name,
            string description,
            Func<T, CommandContext, Task> execute,
            Func<T, CommandContext, bool>? canExecute = null)
        {
            Name = name;
            Description = description;
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(T parameter, CommandContext context)
        {
            return _canExecute?.Invoke(parameter, context) ?? true;
        }

        public Task ExecuteAsync(T parameter, CommandContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
