using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCommand.Abstraction
{
    public interface IFileEntry
    {
        string Path { get; }
        long Size { get; }
        DateTime LastModified { get; }
        Stream OpenReadStream();
    }
}