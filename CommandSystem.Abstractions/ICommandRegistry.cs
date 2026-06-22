
namespace CommandSystem.Abstractions
{
    public interface ICommandRegistry
    {
        void Register(IRegisteredCommand command);
        
        void Unregister(string commandName);
        
        void UnregisterAll();

        IRegisteredCommand? Get(string commandName);
        
        IReadOnlyCollection<IRegisteredCommand> GetAll();
    }
}
