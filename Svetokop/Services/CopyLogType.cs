using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svetokop.Services
{
    public enum CopyLogType
    {
        Info,
        Error,
        Warning,
        Paused,
        Resumed,
        Cancelled,
        FileStarted,
        FileCompleted,
        FileProgress
    }
}
