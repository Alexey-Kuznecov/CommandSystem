
namespace CommandSystem.Commands.UtilProcess
{
    using CommandSystem.Console.Core;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The task manager.
    /// </summary>
    public class TaskManager
    {
        private static readonly HashSet<string> ProtectedProcesses = new(StringComparer.OrdinalIgnoreCase)
        {
            "explorer",
            "csrss",
            "winlogon",
            "dwm",
            "services",
            "system",
            "conhost"
        };

        private static bool IsProtected(Process process)
        {
            return ProtectedProcesses.Contains(process.ProcessName);
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        public static bool ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);
            if (IsProtected(process))
            {
                Console.WriteLine($"❌ Нельзя возобновить защищённый системный процесс: {process.ProcessName}");
                return false;
            }

            foreach (ProcessThread thread in process.Threads)
            {
                IntPtr handle = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (handle == IntPtr.Zero)
                    continue;

                while (ResumeThread(handle) > 0) { /* loop to fully resume */ }
                CloseHandle(handle);
            }

            Console.WriteLine($"✅ Процесс {process.ProcessName} (PID {pid}) возобновлён.");
            return true;
        }

        public static bool SuspendProcess(int pid, bool force = false)
        {
            var process = Process.GetProcessById(pid);
            if (IsProtected(process) && !force)
            {
                Console.WriteLine($"❌ Нельзя приостановить защищённый системный процесс: {process.ProcessName}");
                return false;
            }

            if (IsProtected(process) && force)
            {
                Console.Write($"⚠️ Вы точно хотите приостановить {process.ProcessName}? [y/N]: ");
                string? input = Console.ReadLine();
                if (!string.Equals(input, "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("⛔ Операция отменена.");
                    return true;
                }
            }

            foreach (ProcessThread thread in process.Threads)
            {
                IntPtr handle = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (handle == IntPtr.Zero)
                    continue;

                SuspendThread(handle);
                CloseHandle(handle);
            }
           
            Console.WriteLine($"⏸️ Процесс {process.ProcessName} (PID {pid}) приостановлен.");
            return true;
        }


        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(ThreadAccess desiredAccess, bool inheritHandle, uint threadId);
        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr handle);
        [DllImport("kernel32.dll")]
        private static extern int ResumeThread(IntPtr handle);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);
    }
}