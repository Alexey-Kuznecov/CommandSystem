
namespace CommandSystem.Abstractions
{
    public interface IAsyncCommand<T> : ICommandBase
    {
        Task ExecuteAsync(T parameter, CommandContext context, CancellationToken cancellationToken);
        bool CanExecute(T parameter, CommandContext context);
    }
}
