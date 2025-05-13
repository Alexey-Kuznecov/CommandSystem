using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public class FileCopySuccessContext
    {
        public string SourcePath { get; }
        public string DestinationPath { get; }
        public long FileSize { get; }
        public TimeSpan Duration { get; }

        public FileCopySuccessContext(string source, string destination, long fileSize, TimeSpan duration)
        {
            SourcePath = source;
            DestinationPath = destination;
            FileSize = fileSize;
            Duration = duration;
        }
    }
}
