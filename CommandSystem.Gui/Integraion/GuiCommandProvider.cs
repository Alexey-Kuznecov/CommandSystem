using CommandSystem.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommandSystem.Gui.Integraion
{
    public class GuiCommandProvider : IGuiCommandProvider
    {
        private readonly ICommandDispatcher _dispatcher;

        public GuiCommandProvider(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public IRegisteredCommand? Get(string commandName) => _dispatcher.Get(commandName);
        public IReadOnlyCollection<IRegisteredCommand> GetAll() => _dispatcher.GetAll();
    }
}
