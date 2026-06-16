
using System.Xml.Linq;

namespace CommandSystem.Abstractions
{
    public class CommandMetadata
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
        public List<Type> ContextTypes { get; set; } = new();
        public string? IconKey { get; set; }
        public string? Hotkey { get; set; }
        public bool SupportsUndo { get; set; } = false;

        public CommandMetadata(string id, string description)
        {
            Id = id;
            Name = id;
            Description = description;
        }
    }
}
