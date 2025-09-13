
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Filtering
{
    public class AttributeFileFilter : IFileFilter
    {
        private readonly IReadOnlyList<FileAttributes> _attributes;
        private readonly bool _include;
        private readonly bool _matchAll; // true = AND, false = OR

        public AttributeFileFilter(IEnumerable<FileAttributes> attributes, bool include = true, bool matchAll = false)
        {
            _attributes = attributes.ToList();
            _include = include;
            _matchAll = matchAll;
        }

        public bool ShouldCopy(string filePath)
        {
            var info = new FileInfo(filePath);
            var fileAttrs = info.Attributes;

            bool match;

            if (_matchAll)
            {
                // Файл должен содержать все указанные атрибуты
                match = _attributes.All(attr => (fileAttrs & attr) == attr);
            }
            else
            {
                // Файл должен содержать хотя бы один из указанных атрибутов
                match = _attributes.Any(attr => (fileAttrs & attr) == attr);
            }

            return _include ? match : !match;
        }
    }
}
