using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
{
    public interface ICommand : ICommandBase
    {
        void Execute(CommandContext context);
        bool CanExecute(CommandContext context);
    }
}
