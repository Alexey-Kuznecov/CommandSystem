
namespace CommandSystem.Abstractions;

public interface ICommandBase
{
    string Name { get; }
    string Description { get; }
}