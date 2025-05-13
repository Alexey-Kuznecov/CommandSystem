using IOCommand.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCommand
{
    public class CopySettings
    {
        public bool OverwriteExistingFiles { get; set; }
        public bool PreserveTimestamps { get; set; }
        public bool VerifyAfterCopy { get; set; }
        public List<ICopyFilter> Filters { get; } = new();
    }
}
