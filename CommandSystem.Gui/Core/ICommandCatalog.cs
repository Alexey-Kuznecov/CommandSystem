
using CommandSystem.Abstractions;

namespace CommandSystem.Gui.Core
{
    public interface ICommandCatalog
    {
        void RegisterCommand(string name, ICommand command, object owner);
        ICommand? GetCommand(string name);
    }
}
