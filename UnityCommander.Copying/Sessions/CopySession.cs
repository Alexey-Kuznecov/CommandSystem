
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Settings;

namespace UnityCommander.Copying.Sessions
{
    public class CopySession
    {
        private readonly Dictionary<string, FileCopyItem> _files = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<FileCopyErrorContext> _errors = new();
        private readonly List<FileCopySuccessContext> _successes = new();
        public IReadOnlyList<FileCopyErrorContext> Errors => _errors;
        public IReadOnlyList<FileCopySuccessContext> Successes => _successes;
        public Guid Id { get; } = Guid.NewGuid();
        public string SourcePath { get; }
        public string TargetPath { get; }
        public CopyOptions Options { get; }
        public SessionState State { get; internal set; } = SessionState.Idle;
        public long BytesCopied { get; internal set; }
        public long TotalBytes { get; internal set; }
        public int FilesCopied { get; internal set; }
        public int TotalFiles { get; internal set; }
        public DateTime StartTime { get; internal set; }
        public DateTime? EndTime { get; internal set; }
        public int ProgressStep { get; set; } // шаг прогресса в процентах (1% по умолчанию)
        public bool VerboseLogging { get; set; }

        public CopySession(string source, string destination)
        {
            SourcePath = source ?? throw new ArgumentNullException(nameof(source));
            TargetPath = destination ?? throw new ArgumentNullException(nameof(destination));
        }

        internal void AddError(FileCopyErrorContext error) => _errors.Add(error);
        internal void AddSuccess(FileCopySuccessContext success) => _successes.Add(success);

        

        internal void AddFile(FileCopyItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // перезаписываем, если уже есть такой source
            _files[item.Source] = item;
        }

        public FileCopyItem GetFile(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Source path cannot be null or empty.", nameof(source));

            return _files.TryGetValue(source, out var item) ? item : null;
        }
    }
}
