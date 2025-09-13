using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Filtering
{
    public class SizeFileFilter : IFileFilter
    {
        private readonly long? _minSize;
        private readonly long? _maxSize;
        private readonly bool _include;

        public SizeFileFilter(long? minSize, long? maxSize, bool include = true)
        {
            _minSize = minSize;
            _maxSize = maxSize;
            _include = include;
        }

        public bool ShouldCopy(string filePath)
        {
            var info = new FileInfo(filePath);
            long size = info.Length;

            bool match = true;
            if (_minSize.HasValue && size < _minSize.Value) match = false;
            if (_maxSize.HasValue && size > _maxSize.Value) match = false;

            return _include ? match : !match;
        }
    }
}
