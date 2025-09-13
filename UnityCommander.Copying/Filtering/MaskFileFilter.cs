
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Filtering
{
    public class MaskFileFilter : IFileFilter
    {
        private readonly IReadOnlyList<string> _masks;
        private readonly bool _include; // true = включаем, false = исключаем

        public MaskFileFilter(IEnumerable<string> masks, bool include = true)
        {
            _masks = masks.ToList();
            _include = include;
        }

        public bool ShouldCopy(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (fileName == null)
                return false;

            // Проверяем, совпадает ли файл хотя бы с одной маской
            bool match = _masks.Any(mask => FilePatternMatcher.Match(fileName, mask));

            // Если фильтр "включающий" → возвращаем совпадение
            // Если "исключающий" → возвращаем true только если НЕ совпало
            return _include ? match : !match;
        }
    }
}
