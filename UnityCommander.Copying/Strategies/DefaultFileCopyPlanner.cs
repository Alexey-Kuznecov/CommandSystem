
using System.Runtime.CompilerServices;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
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
            IFileDiscoveryStrategy discoveryStrategy;

            if (options.UseCategories)
            {
                // сразу используем специализированную стратегию
                discoveryStrategy = new CategorizedFileDiscoveryStrategy(_categorizer);
            }
            else
            {
                // если явно задана стратегия — используем её, иначе рекурсивную полную
                discoveryStrategy = options.DiscoveryStrategy ?? new RecursiveFullDiscoveryStrategy();
            }

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
