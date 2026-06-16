
using CommandSystem.Abstractions;
using CommandSystem.Core.Decorators;
using CommandSystem.Core.Execution;

namespace CommandSystem.Core.Factory
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand Create(CommandMetadata meta, Action<CommandContext> handler)
        {
            ICommand cmd = new DelegateCommand(meta.Id, meta.Description, handler);

            if (!string.IsNullOrEmpty(meta.Hotkey))
                cmd = new HotkeyBindableCommandDecorator(cmd, meta.Hotkey);

            if (!string.IsNullOrEmpty(meta.IconKey))
                cmd = new IconCommandDecorator(cmd, meta.IconKey);

            return cmd;
        }

        public IAsyncCommand CreateAsync(
            CommandMetadata meta,
            Func<CommandContext, Task<UndoToken>> execute)
        {
            IAsyncCommand cmd = new AsyncDelegateCommand(meta.Id, meta.Description, execute);

            if (!string.IsNullOrEmpty(meta.Hotkey) || !string.IsNullOrEmpty(meta.IconKey))
                cmd = new CommandMetadataDecorator(cmd, meta.IconKey, meta.Hotkey);

            return cmd;
        }

        public IAsyncCommand CreateAsync(
             CommandMetadata meta,
             Func<CommandContext, Task> execute)
        {
            IAsyncCommand cmd =
                new AsyncDelegateCommand(meta.Id, meta.Description, execute);

            if (!string.IsNullOrEmpty(meta.Hotkey) ||
                !string.IsNullOrEmpty(meta.IconKey))
            {
                cmd = new CommandMetadataDecorator(
                    cmd,
                    meta.IconKey,
                    meta.Hotkey);
            }

            return cmd;
        }

        public IAsyncCommand<T> CreateAsync<T>(
            CommandMetadata meta,
            Func<T, CommandContext, Task> execute)
        {
            IAsyncCommand<T> cmd = new AsyncDelegateCommand<T>(meta.Id, meta.Description, execute);

            if (!string.IsNullOrEmpty(meta.Hotkey) || !string.IsNullOrEmpty(meta.IconKey))
                cmd = new CommandMetadataDecorator<T>(cmd, meta.IconKey, meta.Hotkey);

            return cmd;
        }
    }
}
