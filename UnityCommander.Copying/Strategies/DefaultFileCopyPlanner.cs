using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Filtering;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Strategies
{
    public class DefaultFileCopyPlanner : IFileCopyPlanner
    {
        public async Task<IEnumerable<DiscoveredItem>> GetDiscoveredItems(
            string sourceDirectory,
            string destinationDirectory,
            CopyOptions options,
            CancellationToken cancellationToken)
        {
            // Используем стратегию обнаружения файлов и директорий
            var discoveryStrategy = options.DiscoveryStrategy ?? new RecursiveFullDiscoveryStrategy();

            // Получаем все найденные элементы (файлы и директории)
            var discoveredItems = discoveryStrategy.Discover(sourceDirectory, destinationDirectory).ToList();

            var result = new List<DiscoveredItem>();

            foreach (var item in discoveredItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Пропускаем файлы, не прошедшие фильтр (если фильтр задан)
                if (item.Type == DiscoveredItemType.File && options.FileFilter != null)
                {
                    if (!options.FileFilter.ShouldCopy(item.Source))
                        continue;
                }

                // Пропускаем директории, если выключена опция сохранения пустых папок
                if (item.Type == DiscoveredItemType.Directory)
                {
                    if (!options.AllowEmptyDirectories && !item.HasFilesInside)
                    {
                        // реально пустая директория — пропускаем
                        continue;
                    }
                }

                result.Add(item);
            }

            return result;
        }
    }
}
