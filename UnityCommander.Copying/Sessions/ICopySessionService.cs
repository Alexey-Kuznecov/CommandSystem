using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Sessions
{
    public interface ICopySession
    {
        public void Pause();
        public void Resume();
        public void WaitIfPaused();
        public void Cancel();
    }
}
