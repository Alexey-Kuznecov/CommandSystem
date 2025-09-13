using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Sessions
{
    public enum FileCopyStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed
    }

    public class FileCopyItem
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public long Size { get; set; }
        public long BytesCopied { get; set; }
        public long Progress { get; set; } = 0;
        public FileCopyStatus Status { get; set; }

        public FileCopyItem(string source, string destination)
        {
            Source = source;
            Destination = destination;
            Size = new FileInfo(source).Length;
            Status = FileCopyStatus.Pending;
        }
    }
}
