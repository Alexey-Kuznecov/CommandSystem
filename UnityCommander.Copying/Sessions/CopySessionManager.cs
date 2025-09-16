
using UnityCommander.Copying.Reporting;

namespace UnityCommander.Copying.Sessions
{
    public class CopySessionManager
    {
        private readonly List<CopySessionService> _sessions = new ();
        private readonly ICopyFileReporter _reporter;
        private readonly ICopyLogReporter _logReporter;
        public IReadOnlyList<CopySessionService> Sessions => _sessions.AsReadOnly();
        public CopySessionService? CurrentSession { get; set; }
        public event EventHandler<SessionState>? CurrentSessionStateChanged;

        public CopySessionManager(ICopyFileReporter reporter, ICopyLogReporter logReporter)
        {
            _logReporter = logReporter;
            _reporter = reporter;
        }

        public CopySessionService CreateSession(string source, string target)
        {
            var session = new CopySessionService(source, target, _reporter, _logReporter);
            session.StateChanged += (s, state) => CurrentSessionStateChanged?.Invoke(s, state);
            _sessions.Add(session);
            CurrentSession = session;
            return session;
        }
    }
}
