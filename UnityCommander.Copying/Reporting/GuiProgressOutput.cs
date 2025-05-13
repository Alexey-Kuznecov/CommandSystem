using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Reporting
{
    public class GuiProgressOutput : IProgressOutput
    {
        private readonly Action<string> _updateCallback;  // Пример: действие для обновления UI компонента

        public GuiProgressOutput(Action<string> updateCallback)
        {
            _updateCallback = updateCallback;
        }

        public void Write(string message)
        {
            _updateCallback(message);  // Например, обновление TextBox или выполнение другого действия
        }
    }
}
