using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions.Metadata
{
    public interface IHasHotkey
    {
        string? HotkeyGesture { get; }
    }
}
