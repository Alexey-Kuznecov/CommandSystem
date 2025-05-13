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
        public async Task<IEnumerable<(string Source, string Destination)>> GetFilesToCopyAsync(
            string sourceDirectory,
            string destinationDirectory,
            CopyOptions options,
            CancellationToken cancellationToken)
        {
            //// Сбор всех файлов с учётом рекурсии
            //var filesToCopy = Directory.GetFiles(sourceDirectory, "*", options.IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            // Получаем все файлы, включая пустые папки
            var filesToCopy = Directory.GetFiles(sourceDirectory, "*", options.IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                                        .ToList();

            // Добавляем пустые папки, если нужно
            if (options.AllowEmptyDirectories)
                EnsureEmptyDirectoriesExist(filesToCopy, sourceDirectory, destinationDirectory);

            // Фильтрация файлов
            if (options.FileFilter != null)
            {
                filesToCopy = filesToCopy.Where(file => options.FileFilter.ShouldCopy(file)).ToList();
            }

            // Возвращаем пути файлов с их назначением
            return filesToCopy.Select(file => (file, Path.Combine(destinationDirectory, Path.GetFileName(file))));
        }

        public void EnsureEmptyDirectoriesExist(IEnumerable<string> files, string sourceDirectory, string destinationDirectory)
        {
            // Получаем все директории из исходных путей, включая пустые
            var directories = files.Select(file => Path.GetDirectoryName(Path.Combine(sourceDirectory, Path.GetFileName(file))))
                                   .Distinct();

            // Добавляем все пустые директории из исходного пути
            var emptyDirs = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories)
                                     .Where(dir => !Directory.GetFiles(dir).Any() && !Directory.GetDirectories(dir).Any())
                                     .Distinct();

            // Объединяем директории, которые должны быть созданы
            directories = directories.Concat(emptyDirs).Distinct();

            // Создаём только пустые директории в целевом пути
            foreach (var dir in directories)
            {
                var targetDir = Path.Combine(destinationDirectory, Path.GetRelativePath(sourceDirectory, dir));

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir); // Создаём пустую папку, если её нет
                }
            }
        }
    }
}
