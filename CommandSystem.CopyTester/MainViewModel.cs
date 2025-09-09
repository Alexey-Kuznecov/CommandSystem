
using CommandSystem.CopyTester.ViewModels;
using CommandSystem.Gui.MVVM;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

using UnityCommander.Copying;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Reporting;

namespace CommandSystem.CopyTester
{
    public class MainViewModel : ObservableObject
    {
        private ObservableObject _currentViewModel = null!; // Initialize to avoid nullability warning
        public CopySetupViewModel SetupVM { get; }
        public CopyProgressViewModel ProgressVM { get; }
        public CopyReportViewModel ReportVM { get; }
        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public MainViewModel(CopySessionService sessionService, IProgressReporter reporter, CopyManager copyManager)
        {
            SetupVM = new CopySetupViewModel(sessionService, copyManager, this);
            ProgressVM = new CopyProgressViewModel(sessionService, reporter, this);
            ReportVM = new CopyReportViewModel(sessionService, this);

            CurrentViewModel = SetupVM; // стартовое окно
        }
    }
}
