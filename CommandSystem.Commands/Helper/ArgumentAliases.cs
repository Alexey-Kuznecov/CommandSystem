using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Commands.Helper
{
    public class ArgumentAliases
    {
        private static readonly Dictionary<string, string> _longToShortAliases = new()
        {
            { "--action", "-a" },
            { "--help", "-h" },
            { "--verbose", "-v" }
            // и так далее для других аргументов
        };

        private static readonly Dictionary<string, string> _shortToLongAliases = _longToShortAliases
            .ToDictionary(kv => kv.Value, kv => kv.Key);

        public static string GetFullArgument(string argument)
        {
            // Если это сокращённый аргумент, возвращаем полное имя
            if (_shortToLongAliases.ContainsKey(argument))
            {
                return _shortToLongAliases[argument];
            }

            // Если это уже полное имя, возвращаем как есть
            return argument;
        }

        public static string GetShortArgument(string argument)
        {
            // Если это полное имя, возвращаем сокращение
            if (_longToShortAliases.ContainsKey(argument))
            {
                return _longToShortAliases[argument];
            }

            // Если это уже сокращение, возвращаем как есть
            return argument;
        }
    }
}
