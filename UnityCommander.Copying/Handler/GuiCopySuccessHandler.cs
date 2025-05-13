using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public class GuiCopySuccessHandler : ICopySuccessHandler
    {
        private readonly List<FileCopySuccessContext> _successes = new();
        public IReadOnlyList<FileCopySuccessContext> Successes => _successes;

        public void HandleSuccess(FileCopySuccessContext context)
        {
            _successes.Add(context);
            // Здесь ты можешь добавить визуальное уведомление, лог, иконку галочки и т.п.
        }
    }
}
