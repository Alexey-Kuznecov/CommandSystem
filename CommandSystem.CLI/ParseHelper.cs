
using System.Text.RegularExpressions;

namespace CommandSystem.CLI
{
    public class ParseHelper
    {
        public static string[] ParseArguments(string inputLine)
        {
            // Регулярное выражение для захвата аргументов (с пробелами внутри кавычек или без)
            var regex = new Regex(@"[\""]([^\""]+)[\""]|(\S+)", RegexOptions.Compiled);
            var matches = regex.Matches(inputLine);

            return matches.Cast<Match>()
                          .Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value)
                          .ToArray();
        }
    }
}
