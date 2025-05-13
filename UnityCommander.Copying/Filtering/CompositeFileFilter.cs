
namespace UnityCommander.Copying.Filtering
{
    public class CompositeFileFilter : IFileFilter
    {
        private readonly IEnumerable<IFileFilter> _filters;

        public CompositeFileFilter(IEnumerable<IFileFilter> filters)
        {
            _filters = filters;
        }

        public bool ShouldCopy(string filePath)
        {
            return _filters.All(f => f.ShouldCopy(filePath));
        }
    }
}
