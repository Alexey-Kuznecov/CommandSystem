
namespace UnityCommander.Copying.Filtering
{
    using System.Text.RegularExpressions;

    public static class FilePatternMatcher
    {
        public static bool Match(string fileName, string pattern)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(pattern))
                return false;

            // Преобразуем маску вида "*.dll" в регулярное выражение
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".") + "$";

            return Regex.IsMatch(fileName, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
