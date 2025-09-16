using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Sessions
{
    public class CopySessionController : ICopySessionController
    {
        private readonly ManualResetEventSlim _pauseEvent = new(true);
        private CancellationTokenSource? _cts;

        private SessionState _state;
        public event EventHandler<SessionState>? StateChanged;

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCancelled { get; private set; }

        public SessionState State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    StateChanged?.Invoke(this, _state);
                }
            }
        }

        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        public void Start(long totalBytes, int totalFiles)
        {
            State = SessionState.Running;
            IsRunning = true;
            IsPaused = false;
            IsCancelled = false;
            _cts = new CancellationTokenSource();
        }

        public void Pause()
        {
            State = SessionState.Paused;
            IsPaused = true;
            _pauseEvent.Reset();
        }

        public void Resume()
        {
            State = SessionState.Running;
            IsPaused = false;
            _pauseEvent.Set();
        }

        public void Cancel()
        {
            State = SessionState.Cancelled;
            IsCancelled = true;
            _cts?.Cancel();
        }

        public void Complete()
        {
            State = SessionState.Completed;
            IsRunning = false;
        }

        public void WaitIfPaused()
        {
            _pauseEvent.Wait();
            if (_cts?.Token.IsCancellationRequested ?? false)
                _cts.Token.ThrowIfCancellationRequested();
        }
    }
}
