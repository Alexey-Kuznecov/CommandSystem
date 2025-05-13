using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public class GuiCopyErrorHandler : ICopyErrorHandler
    {
        private readonly List<FileCopyErrorContext> _errors = new();
        public IReadOnlyList<FileCopyErrorContext> Errors => _errors;

        public bool HandleError(FileCopyErrorContext context)
        {
            _errors.Add(context);

            // можно показать пользователю диалог или решение о продолжении
            return true; // допустим, продолжаем
        }
    }
}
