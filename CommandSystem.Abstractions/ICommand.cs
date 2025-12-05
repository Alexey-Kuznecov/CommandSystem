
namespace CommandSystem.Abstractions
{
    public interface ICommand : ICommandBase
    {
        void Execute(CommandContext context);
        bool CanExecute(CommandContext context);
    }
}
