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

            if (double.IsNaN(estimated.TotalSeconds) || double.IsInfinity(estimated.TotalSeconds))
            {
                estimated = TimeSpan.Zero; // неизвестно — ставим 0 или "—"
            }
            else
            {
                // жёсткий лимит на адекватность (например, 365 дней)
                var cappedSeconds = Math.Min(estimated.TotalSeconds, TimeSpan.FromDays(365).TotalSeconds);

                if (estimated.TotalMinutes >= 1)
                {
                    estimated = TimeSpan.FromSeconds(Math.Round(cappedSeconds / 10.0) * 10);
                }
                else if (estimated.TotalSeconds >= 10)
                {
                    estimated = TimeSpan.FromSeconds(Math.Round(cappedSeconds / 5.0) * 5);
                }
                else
                {
                    estimated = TimeSpan.FromSeconds(Math.Round(cappedSeconds)); // мелкое — просто в секундах
                }
            }

            _lastDisplayValue = estimated;
            return estimated;
        }
    }
}
