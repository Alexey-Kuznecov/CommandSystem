
using CommandSystem.Abstractions;

namespace CommandSystem.Gui.Integraion
{
    public class CommandDefinition
    {
        public CommandMetadata? Metadata { get; set; }

        public Func<CommandContext, Task>? Execute { get; set; }

        public Func<CommandContext, Task<UndoToken?>>? UndoExecute { get; set; }

        public bool IsUndoable => UndoExecute != null;
    }
}
