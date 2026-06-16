
namespace CommandSystem.Abstractions
{
    public interface ICommandRegister
    {
        void Register(CommandMetadata metadata, Action<CommandContext> execute);

        void Register(CommandMetadata metadata, Func<CommandContext, Task> execute);

        void Register(CommandMetadata metadata, Func<CommandContext, Task<UndoToken>> executeAsync);

        void Register<TParam>(CommandMetadata metadata, Func<TParam, CommandContext, Task> executeAsync);
    }
}
