using AlexeyKuznetsov.Logger;
using CommandSystem.Console.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Handler
{
    public class LoggerCopyErrorHandler : ICopyErrorHandler
    {
        private ILogger _logger;

        public LoggerCopyErrorHandler(ILogger logger)
        {
            _logger = logger;
        }

        public bool HandleError(FileCopyErrorContext context)
        {
            _logger.LogInfo($"Пропущен файл: '{context.SourcePath}'\nПричина: {context.Exception.Message}");
            return true; // продолжать
        }
    }
}
