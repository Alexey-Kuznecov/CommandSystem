using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
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
