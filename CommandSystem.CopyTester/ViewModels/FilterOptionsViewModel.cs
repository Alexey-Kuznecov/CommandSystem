using CommandSystem.Gui.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Filtering;

namespace CommandSystem.CopyTester.ViewModels
{
    public class FilterOptionsViewModel : ObservableObject
    {
        // Маски
        private string _includeMasks;
        public string IncludeMasks
        {
            get => _includeMasks;
            set => SetProperty(ref _includeMasks, value);
        }

        private string _excludeMasks;
        public string ExcludeMasks
        {
            get => _excludeMasks;
            set => SetProperty(ref _excludeMasks, value);
        }

        // Регулярки
        private string _includeRegex;
        public string IncludeRegex
        {
            get => _includeRegex;
            set => SetProperty(ref _includeRegex, value);
        }

        private string _excludeRegex;
        public string ExcludeRegex
        {
            get => _excludeRegex;
            set => SetProperty(ref _excludeRegex, value);
        }

        // Дата
        public DateTime? MinModifiedDate { get; set; }
        public DateTime? MaxModifiedDate { get; set; }

        // Размер
        public long? MinFileSize { get; set; }
        public long? MaxFileSize { get; set; }

        // Атрибуты
        public bool ExcludeHidden { get; set; }
        public bool ExcludeSystem { get; set; }
        public bool ExcludeReadOnly { get; set; }

        // Режим объединения фильтров
        public FilterMode Mode { get; set; } = FilterMode.And;

        // Преобразовать в FilterOptions для фабрики
        public FilterOptions ToFilterOptions()
        {
            return new FilterOptions
            {
                IncludeMasks = string.IsNullOrWhiteSpace(IncludeMasks)
                    ? new List<string>()
                    : IncludeMasks.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList(),
                ExcludeMasks = string.IsNullOrWhiteSpace(ExcludeMasks)
                    ? new List<string>()
                    : ExcludeMasks.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList(),
                IncludeRegexPatterns = string.IsNullOrWhiteSpace(IncludeRegex)
                    ? new List<string>()
                    : IncludeRegex.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList(),
                ExcludeRegexPatterns = string.IsNullOrWhiteSpace(ExcludeRegex)
                    ? new List<string>()
                    : ExcludeRegex.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList(),

                MinModifiedDate = MinModifiedDate,
                MaxModifiedDate = MaxModifiedDate,

                MinFileSize = MinFileSize,
                MaxFileSize = MaxFileSize,

                ExcludeHidden = ExcludeHidden,
                ExcludeSystem = ExcludeSystem,
                ExcludeReadOnly = ExcludeReadOnly,

                Mode = Mode
            };
        }
    }
}
