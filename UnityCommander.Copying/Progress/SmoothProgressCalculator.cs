using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public class SmoothProgressCalculator // : IProgressCalculator
    {
        private readonly double _threshold;
        private double _lastProgress;

        public SmoothProgressCalculator(double threshold = 0.1)
        {
            _threshold = threshold;
            _lastProgress = 0.0;
        }

        public double Calculate(long totalBytes, long copiedBytes, int totalFiles, int copiedFiles)
        {
            double bytePart = totalBytes > 0 ? (double)copiedBytes / totalBytes : 0.0;
            double filePart = totalFiles > 0 ? (double)copiedFiles / totalFiles : 0.0;

            // Вес: 80% байты, 20% файлы
            double combinedProgress = (bytePart * 0.8 + filePart * 0.2) * 100.0;

            // Ограничим максимум
            combinedProgress = Math.Min(combinedProgress, 100.0);

            // Сглаживание: если прирост больше порога — обновляем, иначе возвращаем последнее значение
            if (combinedProgress - _lastProgress >= _threshold || combinedProgress >= 100.0)
            {
                _lastProgress = combinedProgress;
            }

            return _lastProgress;
        }
    }
}
