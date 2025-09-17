
using UnityCommander.Copying.Reporting;

namespace UnityCommander.Copying.Sessions
{
    public class CopySessionManager
    {
        private readonly List<CopySessionService> _sessions = new ();
        private readonly ICopyReporter _reporter;
        private readonly ICopyReporter _logReporter;
        public IReadOnlyList<CopySessionService> Sessions => _sessions.AsReadOnly();
        public CopySessionService? CurrentSession { get; set; }
        public event EventHandler<SessionState>? CurrentSessionStateChanged;

        public CopySessionManager(ICopyReporter reporter, ICopyReporter logReporter)
        {
            _logReporter = logReporter;
            _reporter = reporter;
        }

        public CopySessionService CreateSession(string source, string target)
        {
            var cotroller = new CopySessionController();
            var session = new CopySession(source, target);
            var sessionService = new CopySessionService(session, cotroller, _reporter, _logReporter);
            cotroller.StateChanged += (s, state) => CurrentSessionStateChanged?.Invoke(s, state);
            _sessions.Add(sessionService);
            CurrentSession = sessionService;
            return sessionService;
        }
    }
}
