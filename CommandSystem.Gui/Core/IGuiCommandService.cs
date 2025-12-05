
namespace CommandSystem.Gui.Core
{
    public interface IGuiCommandService
    {
        IEnumerable<GuiCommandDescriptor> GetAvailableCommands();
        Task ExecuteAsync(string commandName, object? parameter = null);
    }
}
