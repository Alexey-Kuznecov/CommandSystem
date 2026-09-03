
using UnityCommander.Logging.Contracts;

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
            _logger.Info($"Пропущен файл: '{context.SourcePath}'\nПричина: {context.Exception.Message}");
            return true; // продолжать
        }
    }
}
