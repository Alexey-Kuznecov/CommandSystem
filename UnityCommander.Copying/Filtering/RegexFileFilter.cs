
using System.Text.RegularExpressions;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Filtering
{
    using System.Text.RegularExpressions;

    public class RegexFileFilter : IFileFilter
    {
        private readonly IReadOnlyList<Regex> _patterns;
        private readonly bool _include;

        public RegexFileFilter(IEnumerable<string> regexPatterns, bool include = true)
        {
            _patterns = regexPatterns.Select(p => new Regex(p, RegexOptions.IgnoreCase)).ToList();
            _include = include;
        }

        public bool ShouldCopy(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (fileName == null)
                return false;

            bool match = _patterns.Any(r => r.IsMatch(fileName));
            return _include ? match : !match;
        }
    }
}
