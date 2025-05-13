using CommandSystem.Core.Commands;

namespace CommandSystem.Core.Abstractions;

public interface ICommandBase
{
    string Name { get; }
    string Description { get; }
}