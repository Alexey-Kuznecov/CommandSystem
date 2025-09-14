using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Filtering;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Strategies
{
    public class DefaultFileCopyPlanner : IFileCopyPlanner
    {
        private readonly IFileCategorizer _categorizer;

        public DefaultFileCopyPlanner(IFileCategorizer categorizer)
        {
            _categorizer = categorizer ?? throw new ArgumentNullException(nameof(categorizer));
        }

        // Асинхронный потоковый вариант для producer/consumer
        public async IAsyncEnumerable<DiscoveredItem> GetDiscoveredItemsAsyncEnumerable(
            string sourceDirectory,
            string destinationDirectory,
            CopyOptions options,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var discoveryStrategy = options.DiscoveryStrategy ?? new RecursiveFullDiscoveryStrategy();

            await foreach (var item in discoveryStrategy.DiscoverAsync(sourceDirectory, destinationDirectory, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // фильтрация по расширениям/паттернам/и т.д.
                if (item.Type == DiscoveredItemType.File && options.FileFilter != null)
                {
                    if (!options.FileFilter.ShouldCopy(item.Source))
                        continue;
                }

                // пропускаем пустые директории (если требуется)
                if (item.Type == DiscoveredItemType.Directory)
                {
                    if (!options.AllowEmptyDirectories && !item.HasFilesInside)
                        continue;
                }

                // категоризация делаем здесь (асинхронно) — planner знает про бизнес-логику
                if (options.UseCategories && item.Type == DiscoveredItemType.File)
                {
                    try
                    {
                        var category = await _categorizer.CategorizeAsync(item.FileInfo ?? new FileInfo(item.Source));
                        item.Category = category ?? string.Empty;
                        item.Destination = Path.Combine(destinationDirectory, item.Category, Path.GetFileName(item.Source));
                    }
                    catch
                    {
                        // при ошибке категоризации — fallback в root destination
                        item.Category = string.Empty;
                        item.Destination = Path.Combine(destinationDirectory, Path.GetFileName(item.Source));
                    }
                }

                yield return item;
            }
        }

        // Сохраняем обратную совместимость: собираем всё в список (как раньше)
        public async Task<IEnumerable<DiscoveredItem>> GetDiscoveredItems(
            string sourceDirectory,
            string destinationDirectory,
            CopyOptions options,
            CancellationToken cancellationToken)
        {
            var list = new List<DiscoveredItem>();
            await foreach (var item in GetDiscoveredItemsAsyncEnumerable(sourceDirectory, destinationDirectory, options, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                list.Add(item);
            }
            return list;
        }
    }
}
