using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public interface ICopyErrorHandler
    {
        /// <summary>
        /// Обрабатывает исключение при копировании файла и возвращает, можно ли продолжать.
        /// </summary>
        bool HandleError(FileCopyErrorContext context);
    }
}
