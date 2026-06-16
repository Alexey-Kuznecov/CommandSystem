
using CommandSystem.Abstractions;
using CommandSystem.Infrastructure.Execution;

namespace CommandSystem.Gui.Integraion
{
    public class GuiCommandRegister : ICommandRegister
    {
        private readonly ICommandDispatcher _dispatcher;

        public GuiCommandRegister(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Register(CommandMetadata metadata, Action<CommandContext> execute)
        {
            if (_dispatcher is CommandDispatcher dispatcher)
            {
                dispatcher.Register(metadata, execute);
            }
        }

        public void Register(CommandMetadata metadata, Func<CommandContext, Task> execute)
        {
            if (_dispatcher is CommandDispatcher dispatcher)
            {
                dispatcher.Register(metadata, execute);
            }
        }

        public void Register(CommandMetadata metadata, Func<CommandContext, Task<UndoToken>> executeAsync)
        {
            if (_dispatcher is CommandDispatcher dispatcher)
            {
                dispatcher.Register(metadata, executeAsync);
            }
        }

        public void Register<TParam>(CommandMetadata metadata, Func<TParam, CommandContext, Task> executeAsync)
        {
            if (_dispatcher is CommandDispatcher dispatcher)
            {
                dispatcher.Register(metadata, executeAsync);
            }
        }

        public void Register<TViewModel, TParam>(CommandMetadata metadata, Func<TViewModel, TParam, CommandContext, Task> executeAsync)
        {
            throw new NotImplementedException();
        }
    }
}
