using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Filtering
{
    public class FilterOptions
    {
        public FilterMode Mode { get; set; }

        // Маски
        public IList<string> IncludeMasks { get; set; } = new List<string>();
        public IList<string> ExcludeMasks { get; set; } = new List<string>();

        // Регулярки
        public IList<string> IncludeRegexPatterns { get; set; } = new List<string>();
        public IList<string> ExcludeRegexPatterns { get; set; } = new List<string>();

        // Даты
        public DateTime? MinModifiedDate { get; set; }
        public DateTime? MaxModifiedDate { get; set; }

        // Размер
        public long? MinFileSize { get; set; }
        public long? MaxFileSize { get; set; }

        // Атрибуты
        public bool ExcludeHidden { get; set; }
        public bool ExcludeSystem { get; set; }
        public bool ExcludeReadOnly { get; set; }
    }
}
