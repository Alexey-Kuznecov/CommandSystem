using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Metadata
{
    public class CommandMetadata
    {
        public string? Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? IconPath { get; set; }
        public string? Hotkey { get; set; }
        public bool SupportsUndo { get; set; } = false;

        public CommandMetadata(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
