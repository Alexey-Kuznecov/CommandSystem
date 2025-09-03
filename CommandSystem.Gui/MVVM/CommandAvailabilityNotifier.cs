using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Gui.MVVM
{
    public class CommandAvailabilityNotifier
    {
        public event Action? AvailabilityChanged;

        public void Notify()
        {
            AvailabilityChanged?.Invoke();
        }
    }
}
