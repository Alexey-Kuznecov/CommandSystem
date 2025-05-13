using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public class FileCopyErrorContext
    {
        public string SourcePath { get; }
        public string DestinationPath { get; }
        public Exception Exception { get; }

        public FileCopyErrorContext(string source, string destination, Exception ex)
        {
            SourcePath = source;
            DestinationPath = destination;
            Exception = ex;
        }
    }
}
