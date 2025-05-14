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

        public FileCopySuccessContext(string source, string destination, long fileSize)
        {
            SourcePath = source;
            DestinationPath = destination;
            FileSize = fileSize;
        }
    }
}
