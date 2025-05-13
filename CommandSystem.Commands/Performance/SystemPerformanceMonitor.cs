using System.Diagnostics;

namespace CommandSystem.Commands.Performance
{
    public class SystemPerformanceMonitor : IDisposable
    {
        private readonly PerformanceCounter cpuCounter;
        private readonly PerformanceCounter memoryCounter;
        private readonly PerformanceCounter diskReadCounter;
        private readonly PerformanceCounter diskWriteCounter;
        private readonly PerformanceCounter diskTransferCounter;

        public SystemPerformanceMonitor(string diskInstance = "_Total")
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Reads/sec", diskInstance);
            diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Writes/sec", diskInstance);
            diskTransferCounter = new PerformanceCounter("PhysicalDisk", "Disk Transfers/sec", diskInstance);
        }

        public SystemStats GetStats()
        {
            return new SystemStats
            {
                CpuUsage = cpuCounter.NextValue(),
                AvailableMemoryMb = memoryCounter.NextValue(),
                DiskReadsPerSec = diskReadCounter.NextValue(),
                DiskWritesPerSec = diskWriteCounter.NextValue(),
                DiskTransfersPerSec = diskTransferCounter.NextValue()
            };
        }

        public void Dispose()
        {
            cpuCounter.Dispose();
            memoryCounter.Dispose();
            diskReadCounter.Dispose();
            diskWriteCounter.Dispose();
            diskTransferCounter.Dispose();
        }
    }

    public class SystemStats
    {
        public float CpuUsage { get; set; }
        public float AvailableMemoryMb { get; set; }
        public float DiskReadsPerSec { get; set; }
        public float DiskWritesPerSec { get; set; }
        public float DiskTransfersPerSec { get; set; }

        public override string ToString()
        {
            return $"CPU Usage         : {CpuUsage:F2} %\n" +
                   $"Available Memory  : {AvailableMemoryMb:F2} MB\n" +
                   $"Disk Reads/sec    : {DiskReadsPerSec:F2}\n" +
                   $"Disk Writes/sec   : {DiskWritesPerSec:F2}\n" +
                   $"Disk Transfers/sec: {DiskTransfersPerSec:F2}";
        }
    }
}
