using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCommand.Abstraction
{
    public interface ICopyStrategy
    {
        void Copy(IFileEntry sourceFile, string targetPath, CancellationToken cancellationToken);
    }
}
