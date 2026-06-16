
namespace CommandSystem.Abstractions
{
    public interface ICommandFactory
    {
        ICommand Create(CommandMetadata meta, Action<CommandContext> execute);
        IAsyncCommand CreateAsync(CommandMetadata meta, Func<CommandContext, Task<UndoToken>> execute);
        IAsyncCommand CreateAsync(CommandMetadata meta, Func<CommandContext, Task> execute);
        public IAsyncCommand<T> CreateAsync<T>(CommandMetadata meta, Func<T, CommandContext, Task> execute);
    }
}
