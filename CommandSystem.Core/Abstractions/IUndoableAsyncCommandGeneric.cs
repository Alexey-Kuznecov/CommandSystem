using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
{
    public interface IUndoableAsyncCommand<T> : IAsyncCommand<T>
    {
        Task UndoAsync(CommandContext context);
        bool CanUndo(CommandContext context);
    }
}
