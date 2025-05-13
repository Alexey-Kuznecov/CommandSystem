using CommandSystem.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem.Core.Metadata;

namespace CommandSystem.Infrastructure.Lifecycle
{
    public class RegisteredCommand : IRegisteredCommand
    {
        public string Name => Metadata.Name;
        public CommandMetadata Metadata { get; }
        public object Command { get; }

        public RegisteredCommand(CommandMetadata metadata, object command)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }
    }
}
