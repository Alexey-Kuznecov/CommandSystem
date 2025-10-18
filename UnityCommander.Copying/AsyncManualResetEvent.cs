using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityCommander.Copying
{
    public sealed class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> _tcs;

        public AsyncManualResetEvent(bool initialState = false)
        {
            _tcs = CreateNewTcs(initialState);
        }

        private static TaskCompletionSource<bool> CreateNewTcs(bool set = false)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (set)
                tcs.TrySetResult(true);
            return tcs;
        }

        public Task WaitAsync()
        {
            return _tcs.Task;
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
#if NET6_0_OR_GREATER
            return _tcs.Task.WaitAsync(cancellationToken);
#else
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        var tcs = new TaskCompletionSource<bool>();
        var reg = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

        return Task.WhenAny(_tcs.Task, tcs.Task).Unwrap().ContinueWith(t =>
        {
            reg.Dispose();
            return t;
        }, TaskScheduler.Default).Unwrap();
#endif
        }

        public void Set()
        {
            _tcs.TrySetResult(true);
        }

        public void Reset()
        {
            while (true)
            {
                var tcs = _tcs;
                if (!tcs.Task.IsCompleted)
                    return;

                var newTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                if (Interlocked.CompareExchange(ref _tcs, newTcs, tcs) == tcs)
                    return;
            }
        }
    }
}
