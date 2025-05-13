using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Decorators;
using CommandSystem.Core.Metadata;
using CommandSystem.Core.Execution;

namespace CommandSystem.Core.Factory
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand Create(CommandMetadata meta, Action<CommandContext> handler)
        {
            var baseCommand = new DelegateCommand(meta.Name, meta.Description, handler);

            ICommand result = baseCommand;

            if (meta.SupportsUndo)
                result = new UndoableCommandDecorator(result);

            if (!string.IsNullOrEmpty(meta.Hotkey))
                result = new HotkeyBindableCommandDecorator(result, meta.Hotkey);

            if (!string.IsNullOrEmpty(meta.IconPath))
                result = new IconCommandDecorator(result, meta.IconPath);

            return result;
        }

        public IAsyncCommand CreateAsync(
            CommandMetadata meta,
            Func<CommandContext, Task> execute)
        {
            var baseCommand = new AsyncDelegateCommand(meta.Name, meta.Description, execute);
            IAsyncCommand result = baseCommand;

            // Если поддержка отмены (Undo) нужна
            if (meta.SupportsUndo)
                result = new UndoableAsyncCommandDecorator(result);

            // Добавление метаданных (иконка, хоткей)
            if (!string.IsNullOrEmpty(meta.IconPath) || !string.IsNullOrEmpty(meta.Hotkey))
                result = new CommandMetadataDecorator(result, meta.IconPath, meta.Hotkey);

            return result;
        }

        public IAsyncCommand<T> CreateAsync<T>(
            CommandMetadata meta,
            Func<T, CommandContext, Task> execute)
        {
            // 1. Базовая асинхронная команда с параметром
            var baseCommand = new AsyncDelegateCommand<T>(meta.Name, meta.Description, execute);
            IAsyncCommand<T> result = baseCommand;

            // 2. Добавляем поддержку Undo, если нужно
            if (meta.SupportsUndo)
                result = new UndoableAsyncCommandDecorator<T>(result);

            // 3. Декорируем метаданными (иконка, хоткей)
            if (!string.IsNullOrEmpty(meta.IconPath) || !string.IsNullOrEmpty(meta.Hotkey))
                result = new CommandMetadataDecorator<T>(result, meta.IconPath, meta.Hotkey);

            return result;
        }
    }
}
