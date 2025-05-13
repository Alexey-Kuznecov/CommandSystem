using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public interface ICopySuccessHandler
    {
        void HandleSuccess(FileCopySuccessContext context);
    }
}
