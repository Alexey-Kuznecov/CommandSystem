
namespace CommandSystem.Abstractions
{
    public interface IAsyncCommand : ICommandBase
    {
        // Старый метод без CancellationToken
        Task<UndoToken?> ExecuteAsync(CommandContext context);

        // Новый метод с CancellationToken
        Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken);
        bool CanExecute(CommandContext context);
    }
}
