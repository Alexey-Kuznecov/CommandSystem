using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Copying.Sessions
{
    public interface ICopySessionController
    {
        SessionState State { get; }
        event EventHandler<SessionState>? StateChanged;

        void Start(long totalBytes, int totalFiles);
        void Pause();
        void Resume();
        void Cancel();
        void Complete();

        Task WaitIfPausedAsync(CancellationToken cancellationToken);
        CancellationToken CancellationToken { get; }
    }

}
