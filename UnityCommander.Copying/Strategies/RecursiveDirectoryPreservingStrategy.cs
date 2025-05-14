
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

                // Добавляем директорию
                var relativePath = Path.GetRelativePath(sourceRoot, currentSourcePath);
                var destPath = Path.Combine(destinationRoot, relativePath);

                yield return new DiscoveredItem
                {
                    Source = currentSourcePath,
                    Destination = destPath,
                    Type = DiscoveredItemType.Directory
                };

                // Получаем все поддиректории
                foreach (var dir in Directory.GetDirectories(currentSourcePath))
                {
                    stack.Push(dir); // Погружаемся в глубину
                }

                // Получаем все файлы в текущей директории
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
                        Type = DiscoveredItemType.File
                    };
                }
            }
        }
    }
}
