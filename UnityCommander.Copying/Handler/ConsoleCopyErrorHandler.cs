using CommandSystem.Console.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public class ConsoleCopyErrorHandler : ICopyErrorHandler
    {
        private readonly IConsoleOutput _output;

        public ConsoleCopyErrorHandler(IConsoleOutput output)
        {
            _output = output;
        }

        public bool HandleError(FileCopyErrorContext context)
        {
            _output.WriteWarning($"Пропущен файл: {context.SourcePath}\nПричина: {context.Exception.Message}");
            return true; // продолжать
        }
    }
}
