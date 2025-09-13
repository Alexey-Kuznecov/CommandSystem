
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Filtering
{
    public enum FilterMode
    {
        And,
        Or
    }

    public class CompositeFileFilter : IFileFilter
    {
        private readonly IEnumerable<IFileFilter> _filters;
        private readonly FilterMode _mode;

        public CompositeFileFilter(IEnumerable<IFileFilter> filters, FilterMode mode = FilterMode.And)
        {
            _filters = filters;
            _mode = mode;
        }

        public bool ShouldCopy(string filePath)
        {
            return _mode == FilterMode.And
                ? _filters.All(f => f.ShouldCopy(filePath))
                : _filters.Any(f => f.ShouldCopy(filePath));
        }
    }
}
