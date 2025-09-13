using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Filtering;

namespace UnityCommander.Copying.Helper
{
    public static class FileFilterFactory
    {
        public static IFileFilter Create(FilterOptions? options)
        {
            if (options == null)
                return new AllowAllFileFilter();

            var filters = new List<IFileFilter>();

            // Маски
            if (options.IncludeMasks.Any())
                filters.Add(new MaskFileFilter(options.IncludeMasks, include: true));

            if (options.ExcludeMasks.Any())
                filters.Add(new MaskFileFilter(options.ExcludeMasks, include: false));

            // Регулярки
            if (options.IncludeRegexPatterns.Any())
                filters.Add(new RegexFileFilter(options.IncludeRegexPatterns, include: true));

            if (options.ExcludeRegexPatterns.Any())
                filters.Add(new RegexFileFilter(options.ExcludeRegexPatterns, include: false));

            // Даты
            if (options.MinModifiedDate.HasValue || options.MaxModifiedDate.HasValue)
                filters.Add(new DateFileFilter(options.MinModifiedDate, options.MaxModifiedDate));

            // Размер
            if (options.MinFileSize.HasValue || options.MaxFileSize.HasValue)
                filters.Add(new SizeFileFilter(options.MinFileSize, options.MaxFileSize));

            // Атрибуты
            var attrsToExclude = new List<FileAttributes>();

            if (options.ExcludeHidden)
                attrsToExclude.Add(FileAttributes.Hidden);
            if (options.ExcludeSystem)
                attrsToExclude.Add(FileAttributes.System);
            if (options.ExcludeReadOnly)
                attrsToExclude.Add(FileAttributes.ReadOnly);

            if (attrsToExclude.Any())
            {
                // include = false → это исключающие атрибуты
                // matchAll = false → достаточно совпадения хотя бы одного атрибута
                filters.Add(new AttributeFileFilter(attrsToExclude, include: false, matchAll: false));
            }

            // Если фильтров нет — пропускаем всё
            if (!filters.Any())
                return new AllowAllFileFilter();

            // Если один фильтр — возвращаем напрямую
            if (filters.Count == 1)
                return filters[0];

            // Несколько фильтров — объединяем по Mode
            return new CompositeFileFilter(filters, options.Mode);
        }
    }
}
