using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Filtering
{
    public class DateFileFilter : IFileFilter
    {
        private readonly DateTime? _minDate;
        private readonly DateTime? _maxDate;
        private readonly bool _include;

        public DateFileFilter(DateTime? minDate, DateTime? maxDate, bool include = true)
        {
            _minDate = minDate;
            _maxDate = maxDate;
            _include = include;
        }

        public bool ShouldCopy(string filePath)
        {
            var info = new FileInfo(filePath);
            var date = info.LastWriteTime;

            bool match = true;
            if (_minDate.HasValue && date < _minDate.Value) match = false;
            if (_maxDate.HasValue && date > _maxDate.Value) match = false;

            return _include ? match : !match;
        }
    }
}
