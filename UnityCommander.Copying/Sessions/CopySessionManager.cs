using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public CopySessionManager(ICopyFileReporter reporter, ICopyLogReporter logReporter)
        {
            _logReporter = logReporter;
            _reporter = reporter;
        }

        public CopySessionService CreateSession(string source, string target)
        {
            var session = new CopySessionService(source, target, _reporter, _logReporter);
            _sessions.Add(session);
            CurrentSession = session;
            return session;
        }
    }
}
