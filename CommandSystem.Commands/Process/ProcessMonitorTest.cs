using CommandSystem.Console.Core;
using System.Diagnostics;

namespace CommandSystem.Commands.UtilProcess
{
    public class ProcessMonitorTest
    {
        private static IConsoleOutput _output = new ConsoleOutput();

        public static async Task StartAsync(string processName, IConsoleOutput output, CancellationToken cancellationToken)
        {
            _output = output;
            _output.WriteLine($"Запуск мониторинга процесса {processName}...");
            do
            {
                _output.SetCursorVisible(false);
                await Task.Delay(5000, cancellationToken);  // Асинхронная задержка
                VmcControllerTest(processName);
                _output.SetCursorPosition(0, 1);
            }
            while (true);
        }

        private static void VmcControllerTest(string processName)
        {
            //List<FileSystemInfo> infos;

            //var process = Process.GetProcessesByName(processName).Select(p => p.Id).ToList();
            //if (!process.Any())
            //{
            //    _output.WriteLine($"Процесс с именем {processName} не найден.");
            //    return;
            //}

            //infos = new List<FileSystemInfo>();

            //using (var openFiles = ProcessUtility.GetOpenFilesEnumerator(process[0]))
            //{
            //    while (openFiles.MoveNext())
            //    {
            //        infos.Add(openFiles.Current);
            //    }
            //}

            //infos.Sort(SortPaths);

            //foreach (var fileSystem in infos)
            //{
            //    _output.WriteLine(fileSystem.FullName);
            //}
        }

        private static int SortPaths(FileSystemInfo firstPath, FileSystemInfo secondPath)
        {
            return string.Compare(firstPath.FullName, secondPath.FullName, StringComparison.Ordinal);
        }
    }
}
