using CommandSystem.Abstractions;

namespace CommandSystem.Gui.Integraion
{
    public class GuiCommandProvider : IGuiCommandProvider
    {
        private readonly ICommandDispatcher _dispatcher;

        public GuiCommandProvider(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public IRegisteredCommand? Get(string commandName) => _dispatcher.Get(commandName);
        public IReadOnlyCollection<IRegisteredCommand> GetAll() => _dispatcher.GetAll();
    }
}
