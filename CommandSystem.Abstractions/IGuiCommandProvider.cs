using CommandSystem.Abstractions;

namespace CommandSystem.Gui.Integraion
{
    public interface IGuiCommandProvider
    {
        public IRegisteredCommand? Get(string commandName);
        public IReadOnlyCollection<IRegisteredCommand> GetAll();
    }
}