using CommandSystem.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Gui.Core
{
    public interface ICommandCatalog
    {
        void RegisterCommand(string name, ICommand command, object owner);
        ICommand? GetCommand(string name);
    }
}
