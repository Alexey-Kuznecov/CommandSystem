using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Gui.Core
{
    public interface ICommandViewModelBinder
    {
        void BindCommands(object viewModel);
    }
}
