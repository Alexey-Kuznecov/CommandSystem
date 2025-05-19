using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public class HumanReadableTimeCalculator
    {
        private TimeSpan? _lastDisplayValue;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        public TimeSpan GetDisplayValue(TimeSpan estimated, DateTime now)
        {
            // Обновляем только раз в 3 секунды
            if ((now - _lastUpdateTime).TotalSeconds < 3 && _lastDisplayValue.HasValue)
                return _lastDisplayValue.Value;

            _lastUpdateTime = now;

            // Если осталось больше 1 минуты — округляем до 10 секунд
            if (estimated.TotalMinutes >= 1)
            {
                estimated = TimeSpan.FromSeconds(Math.Round(estimated.TotalSeconds / 10.0) * 10);
            }
            else if (estimated.TotalSeconds >= 10)
            {
                estimated = TimeSpan.FromSeconds(Math.Round(estimated.TotalSeconds / 5.0) * 5);
            }
            else
            {
                estimated = TimeSpan.FromSeconds(Math.Round(estimated.TotalSeconds)); // мелкое — просто в секундах
            }

            _lastDisplayValue = estimated;
            return estimated;
        }
    }
}
