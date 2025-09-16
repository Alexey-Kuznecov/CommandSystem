using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.SystemMetrics
{
    public class FinalCopyReport
    {
        public CopyMetricsSnapshot CopyMetrics { get; init; }
        public DiskSpeedSnapshot DiskMetrics { get; init; }

        public override string ToString()
        {
            return $@"
                ==== Итоговый отчёт ====
                Файлы: {CopyMetrics.TotalFiles}
                Всего байт: {CopyMetrics.TotalBytes:N0}
                Ошибки: {CopyMetrics.ErrorsCount}
                Время: {CopyMetrics.TotalDuration}

                Средняя скорость (по копированию): {CopyMetrics.AverageSpeedBytesPerSec / 1024 / 1024:F2} MB/s
                Средняя скорость (по диску): {DiskMetrics.AverageMBps:F2} MB/s
                Мин. скорость диска: {DiskMetrics.MinMBps:F2} MB/s
                Макс. скорость диска: {DiskMetrics.MaxMBps:F2} MB/s
                ========================
                ";
        }
    }
}
