using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.SystemMetrics
{
    public static class DiskUtils
    {
        public static string ResolvePhysicalDiskInstance(char letter)
        {
            var category = new PerformanceCounterCategory("PhysicalDisk");
            string[] instanceNames = category.GetInstanceNames();
            return instanceNames.FirstOrDefault(name => name.EndsWith($"{char.ToUpperInvariant(letter)}:")) ?? "_Total";
        }

        public static string GetPhysicalDiskInstanceFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "_Total";

            try
            {
                var root = Path.GetPathRoot(path);
                if (!string.IsNullOrWhiteSpace(root) && root.Length >= 2 && root[1] == ':')
                {
                    char driveLetter = char.ToUpperInvariant(root[0]);

                    var category = new PerformanceCounterCategory("PhysicalDisk");
                    string[] instanceNames = category.GetInstanceNames();
                    return instanceNames.FirstOrDefault(name => name.EndsWith($"{driveLetter}:")) ?? "_Total";
                }
            }
            catch
            {
                // Игнорируем любые ошибки, возвращаем "_Total"
            }

            return "_Total";
        }

        public static string? ResolvePhysicalDiskInstanceFromPath(string path)
        {
            try
            {
                var driveLetter = Path.GetPathRoot(path)?.TrimEnd('\\');
                if (string.IsNullOrWhiteSpace(driveLetter))
                    return null;

                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition");
                foreach (var obj in searcher.Get())
                {
                    var partition = (ManagementObject)obj["Antecedent"];
                    var logicalDisk = (ManagementObject)obj["Dependent"];

                    var logicalDiskName = logicalDisk["Name"]?.ToString();
                    if (!string.Equals(logicalDiskName, driveLetter, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Теперь найдём физический диск
                    using var partitionSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                    foreach (var disk in partitionSearcher.Get().Cast<ManagementObject>())
                    {
                        var partitions = disk.GetRelated("Win32_DiskPartition");
                        foreach (var part in partitions)
                        {
                            if (part["DeviceID"]?.ToString() == partition["DeviceID"]?.ToString())
                            {
                                var index = disk["Index"];
                                var volumes = disk.GetRelated("Win32_LogicalDisk")
                                                  .Cast<ManagementObject>()
                                                  .Select(ld => ld["Name"]?.ToString())
                                                  .Where(n => n != null);
                                return $"{index} {string.Join(" ", volumes)}";
                            }
                        }
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки
            }

            return null;
        }
    }
}
