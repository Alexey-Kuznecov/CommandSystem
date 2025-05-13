using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
{
    public interface IAsyncCommand<T> : ICommandBase
    {
        Task ExecuteAsync(T parameter, CommandContext context, CancellationToken cancellationToken);
        bool CanExecute(T parameter, CommandContext context);
    }
}
