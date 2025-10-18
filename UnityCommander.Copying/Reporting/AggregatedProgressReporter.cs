
using System.Threading.Channels;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Reporting
{
    public class AggregatedProgressReporter : IProgressReporter, IDisposable
    {
        private readonly IProgressReporter _inner;
        private readonly TimeSpan _interval;
        private readonly SynchronizationContext _context;
        private readonly object _lock = new();
        private ProgressInfo? _latest;
        private Timer _timer;

        public AggregatedProgressReporter(IProgressReporter inner, TimeSpan? interval = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _interval = interval ?? TimeSpan.FromMilliseconds(100);
            _context = SynchronizationContext.Current ?? new SynchronizationContext();
            _timer = new Timer(OnTimerTick, null, _interval, _interval);
        }

        public event Action<ProgressInfo> ProgressChanged
        {
            add => _inner.ProgressChanged += value;
            remove => _inner.ProgressChanged -= value;
        }

        public void Report(ProgressInfo info)
        {
            lock (_lock)
                _latest = info;
        }

        private void OnTimerTick(object? state)
        {
            ProgressInfo? info;
            lock (_lock)
            {
                info = _latest;
                _latest = null;
            }

            if (info != null)
                _context.Post(_ => _inner.Report(info), null);
        }

        public void Dispose() => _timer.Dispose();
    }
}
