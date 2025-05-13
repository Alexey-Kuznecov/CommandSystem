using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Commands.IO
{
    //[ConsoleCommand( "copyfile", "Копирует файл в указанное место.\n" +
    //                 "Пример использования:\n" +
    //                 "copyfile <sourcePath> <destinationPath> [--overwrite] [--rename]\n\n" +
    //                 "Аргументы:\n" +
    //                 "<sourcePath>       Путь к исходному файлу.\n" +
    //                 "<destinationPath>  Путь назначения (файл или папка).\n" +
    //                 "--overwrite        Перезаписать файл, если он уже существует.\n" +
    //                 "--rename           Если файл существует, сохранить копию с новым именем.", "cpfile", "cf")]
    //public class CopyFileCommand : IConsoleCommand  
    //{
    //    public string Name => "copyfile";
    //    public string Description => "Копирует файл в указанное место с возможностью перезаписи или автопереименования.";
    //    public IEnumerable<string> Aliases => ["cpfile", "cf" ];
    //    public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
    //    {
    //        var args = context.Arguments;
    //        if (args.Length < 2)
    //        {
    //            context.Output.WriteLine("Ошибка: укажите путь источника и путь назначения.");
    //            context.Output.WriteLine("Пример: copyfile <sourcePath> <destinationPath> [--overwrite] [--rename]");
    //            return;
    //        }

    //        var sourcePath = args[0];
    //        var destinationPath = args[1];

    //        // Флаги
    //        bool overwrite = args.Contains("--overwrite", StringComparer.OrdinalIgnoreCase);
    //        bool renameIfExists = args.Contains("--rename", StringComparer.OrdinalIgnoreCase);

    //        if (!File.Exists(sourcePath))
    //        {
    //            context.Output.WriteLine($"Ошибка: файл не найден: {sourcePath}");
    //            return;
    //        }

    //        try
    //        {
    //            if (Directory.Exists(destinationPath))
    //            {
    //                var fileName = Path.GetFileName(sourcePath);
    //                destinationPath = Path.Combine(destinationPath, fileName);
    //            }

    //            if (File.Exists(destinationPath))
    //            {
    //                if (overwrite)
    //                {
    //                    await CopyFileWithProgress(sourcePath, destinationPath, context, cancellationToken);
    //                    context.Output.WriteLine($"\nФайл перезаписан в: {destinationPath}");
    //                }
    //                else if (renameIfExists)
    //                {
    //                    destinationPath = GenerateNewFileName(destinationPath);
    //                    await CopyFileWithProgress(sourcePath, destinationPath, context, cancellationToken);
    //                    context.Output.WriteLine($"\nФайл скопирован с новым именем: {destinationPath}");
    //                }
    //                else
    //                {
    //                    context.Output.WriteLine($"Ошибка: файл уже существует: {destinationPath}");
    //                    context.Output.WriteLine("Используйте флаг --overwrite или --rename для обработки конфликта.");
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                await CopyFileWithProgress(sourcePath, destinationPath, context, cancellationToken);
    //                context.Output.WriteLine($"\nФайл успешно скопирован в: {destinationPath}");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            context.Output.WriteLine($"Ошибка копирования файла: {ex.Message}");
    //        }

    //        await Task.CompletedTask;
    //    }

    //    // Метод для копирования файла с прогресс-баром
    //    public async Task CopyFileWithProgress(string sourcePath, string destinationPath, IConsoleCommandContext context, CancellationToken cancellationToken)
    //    {
    //        var totalBytes = new FileInfo(sourcePath).Length;
    //        var bufferSize = 1024 * 16;  // Размер буфера для чтения файла
    //        var buffer = new byte[bufferSize];

    //        int bytesCopied = 0;
    //        using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
    //        using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
    //        {
    //            int bytesRead;
    //            while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
    //            {
    //                await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
    //                bytesCopied += bytesRead;

    //                // Рассчитываем процент завершенности
    //                double progress = (double)bytesCopied / totalBytes * 100;
    //                DisplayProgressBar(progress, context);
    //            }
    //        }
    //    }

    //    // Метод для отображения прогресс-бара в консоли
    //    public void DisplayProgressBar(double progress, IConsoleCommandContext context)
    //    {
    //        int totalBars = 50;  // Количество символов в прогресс-баре
    //        int progressBars = (int)(progress / 2);  // Определяем количество символов для прогресса
    //        string bar = new string('#', progressBars) + new string('-', totalBars - progressBars);

    //        // Выводим прогресс-бар с процентом
    //        context.Output.Write($"\r[{bar}] {progress:0.00}%");
    //    }


    //    private string GenerateNewFileName(string filePath)
    //    {
    //        var directory = Path.GetDirectoryName(filePath)!;
    //        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
    //        var extension = Path.GetExtension(filePath);

    //        int counter = 1;
    //        string newFilePath;
    //        do
    //        {
    //            newFilePath = Path.Combine(directory, $"{fileNameWithoutExt} ({counter}){extension}");
    //            counter++;
    //        }
    //        while (File.Exists(newFilePath));

    //        return newFilePath;
    //    }

    //    public IEnumerable<string> GetSuggestions(string[] args)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Execute(IConsoleCommandContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
