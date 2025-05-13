
using System.Text.RegularExpressions;

namespace UnityCommander.Copying.Filtering
{
    public class RegexFileFilter : IFileFilter
    {   
        private readonly Regex _regex;

        public RegexFileFilter(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled);
        }

        public bool ShouldCopy(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return fileName != null && _regex.IsMatch(fileName);
        }
    }
}
