
using CommandSystem.Abstractions;

namespace CommandSystem.Infrastructure.Execution
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ICommandHistory? _history;

        public CommandExecutor(ICommandHistory? history = null)
        {
            _history = history;
        }

        public void Execute(ICommand command, CommandContext context)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (!command.CanExecute(context)) return;

            try
            {
                command.Execute(context);

                if (command is IUndoableCommand undoable)
                {
                    _history?.Push(undoable, context);
                }
            }
            catch (Exception ex)
            {
                // Логирование или обработка исключений
                throw new InvalidOperationException("Execution failed.", ex);
            }
        }

        public async Task ExecuteAsync(IAsyncCommand command, CommandContext? context = null, CancellationToken cancellationToken = default)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (!command.CanExecute(context)) return;

            try
            {
                await command.ExecuteAsync(context, cancellationToken);

                if (command is IUndoableCommand undoable)
                {
                    _history?.Push(undoable, context);
                }
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены выполнения
                // Можно логировать, если нужно
            }
            catch (Exception ex)
            {
                // Логирование или обработка других исключений
                throw new InvalidOperationException("Execution failed.", ex);
            }
        }

        public async Task ExecuteAsync<T>(IAsyncCommand<T> command, T parameter, CommandContext? context = null, CancellationToken cancellationToken = default)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (!command.CanExecute(parameter, context)) return;

            try
            {
                await command.ExecuteAsync(parameter, context, cancellationToken);

                if (command is IUndoableCommand undoable)
                {
                    _history?.Push(undoable, context);
                }
            }
            catch (OperationCanceledException)
            {
                // Обработка отмены выполнения
                // Можно логировать, если нужно
            }
            catch (Exception ex)
            {
                // Логирование или обработка других исключений
                throw new InvalidOperationException("Execution failed.", ex);
            }
        }

        public async Task ExecuteAsync(object command, CommandContext context)
        {
            switch (command)
            {
                case IAsyncCommand asyncCommand:
                    if (asyncCommand.CanExecute(context))
                        await asyncCommand.ExecuteAsync(context);
                    break;

                case ICommand syncCommand:
                    if (syncCommand.CanExecute(context))
                        syncCommand.Execute(context);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported command type.");
            }
        }

        public async Task UndoAsync(object command, CommandContext context)
        {
            switch (command)
            {
                case IUndoableAsyncCommand asyncUndoable:
                    await asyncUndoable.UndoAsync(context);
                    break;

                case IUndoableCommand syncUndoable:
                    syncUndoable.Undo(context);
                    break;

                default:
                    throw new InvalidOperationException("Command is not undoable.");
            }
        }
    }
}
