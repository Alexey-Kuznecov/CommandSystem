using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Commands.IO
{
    [ConsoleCommand("copyall", "Копирует все файлы из папки в указанное место.\n" +
                     "Пример использования:\n" +
                     "copyfile <sourcePath> <destinationPath> [--overwrite] [--rename]\n\n" +
                     "Аргументы:\n" +
                     "<sourcePath>       Путь к папке лежат файлы.\n" +
                     "<destinationPath>  Путь назначения (папка).\n" +
                     "--overwrite        Перезаписать файл, если он уже существует.\n" +
                     "--rename           Если файл существует, сохранить копию с новым именем.", "copy", "cpy", "ca" )]
    public class CopyAllCommand : IConsoleCommand
    {
        public string Name => "copyall";
        public string Description => "Копирует все файлы из папки в указанное место.";
        public IEnumerable<string> Aliases => ["copy", "cpy", "ca"];
        public void Execute(IConsoleCommandContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            var args = context.Arguments;
            if (args.Length < 2)
            {
                context.Output.WriteLine("Ошибка: укажите путь источника и путь назначения.");
                context.Output.WriteLine("Пример: copyall <sourceDirectory> <destinationDirectory> [--overwrite] [--rename]");
                return;
            }

            var sourceDirectory = args[0];
            var destinationDirectory = args[1];

            bool overwrite = args.Contains("--overwrite", StringComparer.OrdinalIgnoreCase);
            bool renameIfExists = args.Contains("--rename", StringComparer.OrdinalIgnoreCase);

            if (!Directory.Exists(sourceDirectory))
            {
                context.Output.WriteLine($"Ошибка: папка не найдена: {sourceDirectory}");
                return;
            }

            try
            {
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly);
                if (files.Length == 0)
                {
                    context.Output.WriteLine("Нет файлов для копирования в указанной папке.");
                    return;
                }

                long totalBytes = files.Sum(f => new FileInfo(f).Length);
                long bytesCopied = 0;

                foreach (var sourceFile in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var fileName = Path.GetFileName(sourceFile);
                    var destinationFile = Path.Combine(destinationDirectory, fileName);

                    if (File.Exists(destinationFile))
                    {
                        if (overwrite)
                        {
                            bytesCopied = await CopyFileWithProgress(sourceFile, destinationFile, context, bytesCopied, totalBytes, cancellationToken);
                        }
                        else if (renameIfExists)
                        {
                            destinationFile = GenerateNewFileName(destinationFile);
                            bytesCopied = await CopyFileWithProgress(sourceFile, destinationFile, context, bytesCopied, totalBytes, cancellationToken);
                        }
                        else
                        {
                            context.Output.WriteLine($"Пропущено (файл уже существует): {destinationFile}");
                            var fileInfo = new FileInfo(sourceFile);
                            bytesCopied += fileInfo.Length; // учтем размер пропущенного файла для общего прогресса
                        }
                    }
                    else
                    {
                        bytesCopied = await CopyFileWithProgress(sourceFile, destinationFile, context, bytesCopied, totalBytes, cancellationToken);
                    }
                }

                context.Output.WriteLine("\nКопирование завершено!");
            }
            catch (Exception ex)
            {
                context.Output.WriteLine($"Ошибка копирования файлов: {ex.Message}");
            }
        }

        public IEnumerable<string> GetSuggestions(string[] args)
        {
            throw new NotImplementedException();
        }

        private async Task<long> CopyFileWithProgress(string sourcePath, string destinationPath, IConsoleCommandContext context, long bytesCopied, long totalBytes, CancellationToken cancellationToken)
        {
            var bufferSize = 1024 * 16; // 16 KB
            var buffer = new byte[bufferSize];

            using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                int bytesRead;
                while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    bytesCopied += bytesRead;

                    DisplayProgressBar(context, bytesCopied, totalBytes, sourcePath);
                }
            }

            return bytesCopied;
        }


        private void DisplayProgressBar(IConsoleCommandContext context, long bytesCopied, long totalBytes, string currentFile)
        {
            double progress = (double)bytesCopied / totalBytes * 100;
            int totalBars = 50;
            int progressBars = (int)(progress / 2);
            string bar = new string('#', progressBars) + new string('-', totalBars - progressBars);

            var fileName = Path.GetFileName(currentFile);
            context.Output.Write($"\r[{bar}] {progress:0.00}% (Копирую: {fileName})");
        }

        private string GenerateNewFileName(string path)
        {
            string directory = Path.GetDirectoryName(path)!;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            int counter = 1;
            string newPath;
            do
            {
                newPath = Path.Combine(directory, $"{fileNameWithoutExtension} ({counter}){extension}");
                counter++;
            } while (File.Exists(newPath));

            return newPath;
        }
    }
}
