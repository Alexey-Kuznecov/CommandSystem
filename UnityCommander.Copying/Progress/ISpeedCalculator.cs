using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public interface ISpeedCalculator
    {
        void Reset();
        void Update(long bytesCopiedDelta);
        double GetSpeedBytesPerSecond();
    }
}
