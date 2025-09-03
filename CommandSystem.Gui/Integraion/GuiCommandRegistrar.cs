using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;
using CommandSystem.Gui.Core;
using CommandSystem.Infrastructure.Execution;

namespace CommandSystem.Gui.Integraion
{
    public class GuiCommandRegistrar : ICommandRegister
    {
        private readonly ICommandDispatcher _dispatcher;

        public GuiCommandRegistrar(ICommandDispatcher dispatcher)
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

        public void Register(CommandMetadata metadata, Func<CommandContext, Task> executeAsync)
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
