using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
{
    public interface IAsyncCommand : ICommandBase
    {
        // Старый метод без CancellationToken
        Task ExecuteAsync(CommandContext context);

        // Новый метод с CancellationToken
        Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken);
        bool CanExecute(CommandContext context);
    }
}
