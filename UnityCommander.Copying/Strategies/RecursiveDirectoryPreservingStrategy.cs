
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class RecursiveFullDiscoveryStrategy : IFileDiscoveryStrategy
    {
        public IEnumerable<DiscoveredItem> Discover(string sourceRoot, string destinationRoot)
        {
            if (!Directory.Exists(sourceRoot))
                yield break;

            var stack = new Stack<string>();
            stack.Push(sourceRoot);

            while (stack.Count > 0)
            {
                var currentSourcePath = stack.Pop();
                var relativePath = Path.GetRelativePath(sourceRoot, currentSourcePath);
                var destPath = Path.Combine(destinationRoot, relativePath);

                // Проверяем: есть ли файлы внутри этой папки (только в текущем уровне)
                bool hasFilesInside = Directory.EnumerateFiles(currentSourcePath).Any();

                yield return new DiscoveredItem
                {
                    Source = currentSourcePath,
                    Destination = destPath,
                    Type = DiscoveredItemType.Directory,
                    HasFilesInside = hasFilesInside
                };

                // Получаем поддиректории
                foreach (var dir in Directory.GetDirectories(currentSourcePath))
                    stack.Push(dir);

                // Получаем файлы
                foreach (var file in Directory.GetFiles(currentSourcePath))
                {
                    var relPath = Path.GetRelativePath(sourceRoot, file);
                    var destFilePath = Path.Combine(destinationRoot, relPath);
                    var fileInfo = new FileInfo(file);

                    yield return new DiscoveredItem
                    {
                        Source = file,
                        Destination = destFilePath,
                        FileSize = fileInfo.Length,
                        FileInfo = fileInfo,
                        Type = DiscoveredItemType.File,
                        HasFilesInside = false
                    };
                }
            }
        }
    }
}
