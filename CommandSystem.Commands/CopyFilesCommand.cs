
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Copying;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Filtering;
using UnityCommander.Copying.Settings;

namespace CommandSystem.Commands
{
    [ConsoleCommand("copyfiles", "Копирует файл в указанное место.\n" +
                     "Пример использования:\n" +
                     "copyfile <sourcePath> <destinationPath> [--overwrite] [--rename]\n\n" +
                     "Аргументы:\n" +
                     "<sourcePath>       Путь к исходному файлу.\n" +
                     "<destinationPath>  Путь назначения (файл или папка).\n" +
                     "--overwrite        Перезаписать файл, если он уже существует.\n" +
                     "--rename           Если файл существует, сохранить копию с новым именем.", "cpfile", "cf")]
    public class CopyFilesCommand : IConsoleCommand, IDisposable
    {
        public string Name => "copyfiles";
        public string Description => "Копирует файлы из источника в назначение с поддержкой фильтров и прогресс-бара.";
        public IEnumerable<string> Aliases => ["cpfile", "cf"];
        private readonly CopyManager _copyManager;
        private readonly CopyOptions _copyOptions;
        private IConsoleCommandContext _context;
        public CopyFilesCommand(IServiceProvider serviceProvider)
        {
            _copyManager = serviceProvider.GetRequiredService<CopyManager>();
            _copyOptions = serviceProvider.GetRequiredService<CopyOptions>();

            IFileFilter filter = new CompositeFileFilter(new List<IFileFilter>
            {
                //new MaskFileFilter("*.cs"),  // Пример фильтра по маске, можно передавать через аргументы
                //new RegexFileFilter(@"^\w+\.txt$")  // Пример фильтра по регулярному выражению
            });

            // Подготовка опций копирования
            _copyOptions.FileFilter = filter;
            _copyOptions.UseMultiThreading = true;  // Можно передавать через аргументы, например, --multithread
            _copyOptions.IsRecursive = true;
            _copyOptions.AllowEmptyDirectories = true;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            _context = context;
            var args = context.Arguments;
            var output = context.Output;
            //if (args.Length < 2)
            //{
            //    context.Output.WriteLine("Ошибка: Укажите путь источника и путь назначения.");
            //    return;
            //}

            var sourceDirectory = @"E:\Projects\03._Tests\CopyFileTest\Source3" ?? args[0];
            var destinationDirectory = @"E:\Projects\03._Tests\CopyFileTest\Target" ?? args[1];
            //var sourceDirectory = @"c:\TestCopy\Source" ?? args[0];
            //var destinationDirectory = @"c:\TestCopy\Target" ?? args[1];

            if (string.IsNullOrWhiteSpace(sourceDirectory) || string.IsNullOrWhiteSpace(destinationDirectory))
            {
                output.WriteLine("Ошибка: Укажите путь источника и путь назначения.");
                return;
            }

            if (!Directory.EnumerateFileSystemEntries(sourceDirectory).Any())
            {
                output.WriteLine($"Папка пуста нечего копировать {destinationDirectory}.");
                return;
            }

            if (args.Contains("-t"))
            {
                try
                {
                    using var localCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    var localToken = localCts.Token;

                    while (!localToken.IsCancellationRequested)
                    {
                        var copyTask = _copyManager.CopyFilesAsync(sourceDirectory, destinationDirectory, _copyOptions, localToken);

                        try
                        {
                            await copyTask;
                        }
                        catch (OperationCanceledException)
                        {
                            // Копирование отменено, выходим из цикла
                            break;
                        }

                        ClearDirectory(destinationDirectory);
                        await Task.Delay(100, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    output.WriteLine("Операция отменена.");
                }
            }
            else
            {
                try
                {
                    await _copyManager.CopyFilesAsync(sourceDirectory, destinationDirectory, _copyOptions, cancellationToken);
                    if (args.Contains("-c"))
                    {
                        ClearDirectory(destinationDirectory);
                    }
                }
                catch (OperationCanceledException)
                {
                    output.WriteLine("\n[CopyFilesCommand] Операция отменена.");
                }
            }
        }

        public static void ClearDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Папка не найдена: {path}");

            // Удаляем все файлы
            foreach (var file in Directory.GetFiles(path))
            {
                File.SetAttributes(file, FileAttributes.Normal); // на случай, если файл read-only
                File.Delete(file);
            }

            // Удаляем все подкаталоги рекурсивно
            foreach (var dir in Directory.GetDirectories(path))
            {
                Directory.Delete(dir, true);
            }
        }
        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            var args = _context.Arguments;
            if (args.Contains("-c") && args[1] != null)
                ClearDirectory(args[1]);
        }
    }
}
