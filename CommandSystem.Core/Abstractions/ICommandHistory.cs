using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
{
    public interface ICommandHistory
    {
        void Push(IUndoableCommand command, CommandContext context);
        bool CanUndo { get; }
        bool CanRedo { get; }

        void Undo();
        void Redo();
    }
}
