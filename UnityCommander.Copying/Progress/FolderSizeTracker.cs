using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Progress
{
    public class FolderSizeTracker
    {
        private readonly string _folderPath;
        private long _lastReportedSize = -1;

        public FolderSizeTracker(string folderPath)
        {
            _folderPath = folderPath;
        }

        public long GetSize()
        {
            try
            {
                return Directory.EnumerateFiles(_folderPath, "*", SearchOption.AllDirectories)
                    .Select(path =>
                    {
                        try { return new FileInfo(path).Length; }
                        catch { return 0L; }
                    }).Sum();
            }
            catch
            {
                return 0;
            }
        }

        public string GetFormattedSize() => FormatSize(GetSize());

        private static string FormatSize(long size)
        {
            double mb = size / 1024.0 / 1024.0;
            return $"{mb:F2} MB";
        }
    }
}
