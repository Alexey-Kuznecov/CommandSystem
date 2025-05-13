using CommandSystem.Console.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Commands.IO
{
    /// <summary>
    /// The directory watcher.
    /// </summary>
    public class DirectoryWatcher : IDisposable
    {
        /// <summary>
        /// The watcher.
        /// </summary>
        private static FileSystemWatcher watcher;
        private static IConsoleOutput _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryWatcher"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public DirectoryWatcher(string path, IConsoleOutput output)
        {
            Watcher(path);
            _output = output;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            watcher.Dispose();
        }

        /// <summary>
        /// The watcher.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
#pragma warning disable SYSLIB0003 // Тип или член устарел
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
#pragma warning restore SYSLIB0003 // Тип или член устарел
        private static void Watcher(string path)
        {
            // If a directory is not specified, exit program.
            if (string.IsNullOrEmpty(path))
            {
                // Display the proper way to call the program.
                _output.WriteLine("Usage: Watcher.exe (directory)");
                return;
            }

            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher();
            watcher.Path = path;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Only watch text files.
            watcher.Filter = "*";
            watcher.IncludeSubdirectories = true; // Включаем отслеживание подкаталогов
            watcher.EnableRaisingEvents = true; // Включаем события

            // Add event handlers.
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Changed += OnChanged;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            HandleEvent(e);
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            HandleEvent(e);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Для переименованных файлов выводим старое и новое имя
            _output.WriteLine($"Файл/Папка переименован(а): {e.OldFullPath} -> {e.FullPath}");
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            HandleEvent(e);
        }

        private static void HandleEvent(FileSystemEventArgs e)
        {
            string path = e.FullPath;

            // Проверка, является ли объект файлом или папкой, основываясь на расширении
            string fileExtension = Path.GetExtension(path);

            if (string.IsNullOrEmpty(fileExtension))
            {
                _output.WriteLine($"Папка: {path} {e.ChangeType}");
            }
            else
            {
                _output.WriteLine($"Файл: {path} {e.ChangeType}");
            }
        }
    }
}
