using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Gui.Core
{
    public interface IGuiCommandService
    {
        IEnumerable<GuiCommandDescriptor> GetAvailableCommands();
        Task ExecuteAsync(string commandName, object? parameter = null);
    }
}
