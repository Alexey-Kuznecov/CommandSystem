using CommandSystem.Gui.MVVM;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Sessions;

namespace CommandSystem.CopyTester.ViewModels
{
    public class CopyReportViewModel : ObservableObject
    {
        private readonly MainViewModel _main;
        private readonly CopySessionService _sessionService;
        public IEnumerable<FileCopyErrorContext> Errors => _sessionService.Errors;
        public IEnumerable<FileCopySuccessContext> Successes => _sessionService.Successes;

        public CopyReportViewModel(MainViewModel main)
        {
            _main = main;
        }
    }
}
