using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Settings
{
    public interface ICopySetting
    {
        void Apply(ref CopyOptions options);
    }
}
