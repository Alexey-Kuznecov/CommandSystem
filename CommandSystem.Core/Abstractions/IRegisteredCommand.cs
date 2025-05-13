using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Abstractions
{
    public interface IRegisteredCommand
    {
        string Name { get; }
        object Command { get; }
    }
}
