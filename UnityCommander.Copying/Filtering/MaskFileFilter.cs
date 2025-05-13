
namespace UnityCommander.Copying.Filtering
{
    public class MaskFileFilter : IFileFilter
    {
        private readonly string _mask;

        public MaskFileFilter(string mask)
        {
            _mask = mask;
        }

        public bool ShouldCopy(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            return fileName != null && FilePatternMatcher.Match(fileName, _mask);
        }
    }
}
