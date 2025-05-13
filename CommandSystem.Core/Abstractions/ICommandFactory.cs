using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
{
    public interface ICommandFactory
    {
        ICommand Create(CommandMetadata meta, Action<CommandContext> execute);
        IAsyncCommand CreateAsync(CommandMetadata meta, Func<CommandContext, Task> execute);

        public IAsyncCommand<T> CreateAsync<T>(CommandMetadata meta, Func<T, CommandContext, Task> execute);
    }
}
