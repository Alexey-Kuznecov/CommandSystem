
namespace CommandSystem.Abstractions
{
    public interface IRegisteredCommand
    {
        string Name { get; }
        object Command { get; }
    }
}
