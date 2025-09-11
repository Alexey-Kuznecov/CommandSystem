using CommandSystem.Gui.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying.Handler;

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
