using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Reporting
{
    public interface IProgressOutput
    {
        void Write(string message);
    }
}
