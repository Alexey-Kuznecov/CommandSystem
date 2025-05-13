using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Commands
{
    public class CommandExecutionResult
    {
        public bool Success { get; }
        public string? Message { get; }

        public CommandExecutionResult(bool success, string? message = null)
        {
            Success = success;
            Message = message;
        }

        public static CommandExecutionResult Ok() => new(true);
        public static CommandExecutionResult Fail(string message) => new(false, message);
    }
}
