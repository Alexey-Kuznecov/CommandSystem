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
        public string Name { get; }
        public CommandMetadata Metadata { get; }
        public object Command { get; }

        public RegisteredCommand(CommandMetadata metadata, object command)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (string.IsNullOrWhiteSpace(metadata.Name)) throw new ArgumentException("Metadata name cannot be null or whitespace.", nameof(metadata));

            Name = metadata.Name;
            Metadata = metadata;
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }
    }
}
