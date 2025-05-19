using AlexeyKuznetsov.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying
{
    public class LogHelper
    {
        public ILogger Log { get; }

        public LogHelper()
        {
            Log = new FileLogger();
        }
    }
}
