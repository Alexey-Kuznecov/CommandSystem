
using CommandSystem.CopyTester.ViewModels;
using CommandSystem.Gui.MVVM;
using Spectre.Console;
using System.Collections.ObjectModel;

using UnityCommander.Copying;
using UnityCommander.Copying.Reporting;

namespace CommandSystem.CopyTester
{
    public class MainViewModel : ObservableObject
    {
        private readonly CopyManager _copyManager;
        private readonly IProgressReporter _reporter;
        private ObservableObject _currentViewModel;
        public CopySetupViewModel SetupVM { get; }
        public CopyReportViewModel ReportVM { get; }
        public CopyProgressViewModel ProgressVM { get; private set; }
        public ObservableCollection<CopySessionService> ActiveSessions { get; } = new();
        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public MainViewModel(CopyManager copyManager, IProgressReporter reporter)
        {
            _copyManager = copyManager;
            _reporter = reporter;

            SetupVM = new CopySetupViewModel(this);
            //ReportVM = new CopyReportViewModel(this);

            CurrentViewModel = SetupVM;
        }

        // Добавление сессии в очередь и запуск
        public void StartQuickCopy(string source, string destination)
        {
            var session = new CopySessionService
            {
                SourcePath = source,
                TargetPath = destination
            };
            RunCopySession(session);
        }

        public void StartWizardCopy(CopySessionService session)
        {
            RunCopySession(session);
        }

        private void RunCopySession(CopySessionService session)
        {
            this.ActiveSessions.Add(session);
            this.ProgressVM = new CopyProgressViewModel(session, _reporter, this);
            this.CurrentViewModel = ProgressVM;
            var cts = new CancellationTokenSource();
            session.CancellationToken = cts.Token;
            _ = _copyManager.CopyFilesAsync(session, cts.Token);
        }
    }
}
