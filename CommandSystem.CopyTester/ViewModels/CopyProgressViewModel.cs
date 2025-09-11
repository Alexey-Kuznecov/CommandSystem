
using CommandSystem.Gui.MVVM;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;

namespace CommandSystem.CopyTester.ViewModels
{
    public class CopyProgressViewModel : ObservableObject
    {
        private readonly CopySessionService _sessionService;
        private readonly MainViewModel _main;
        private HumanReadableTimeCalculator _humanCalculator = new HumanReadableTimeCalculator();

        private int _percentage;
        private string _сurrentFileName = string.Empty;
        private string _remainingTime = string.Empty;
        private string _totalCopiedText = string.Empty;
        private string _speedText = string.Empty;
        private string _filesCopiedText = string.Empty;
        private string _overallProgressText = string.Empty;


        public int Percentage
        {
            get => _percentage;
            private set => SetProperty(ref _percentage, value);
        }

        public string CurrentFileName
        {
            get => _сurrentFileName;
            private set => SetProperty(ref _сurrentFileName, value);
        }

        public string RemainingTime
        {
            get => _remainingTime;
            private set => SetProperty(ref _remainingTime, value);
        }
        public string FilesCopiedText
        {
            get => _filesCopiedText;
            private set => SetProperty(ref _filesCopiedText, value);
        }

        public string TotalCopiedText
        {
            get => _totalCopiedText;
            private set => SetProperty(ref _totalCopiedText, value);
        }

        public string SpeedText
        {
            get => _speedText;
            private set => SetProperty(ref _speedText, value);
        }

        public string OverallProgressText
        {
            get => _overallProgressText;
            private set => SetProperty(ref _overallProgressText, value);
        }

        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand CancelCommand { get; }

        public CopyProgressViewModel(CopySessionService sessionService, IProgressReporter reporter, MainViewModel main)
        {
            _sessionService = sessionService;
            _main = main;
            reporter.ProgressChanged += info =>
            {
                this.Percentage = (int)Math.Round(info.CompletionPercentage);
                this.CurrentFileName = info.CurrentFile ?? string.Empty;
                this.SpeedText = $"{info.SpeedBytesPerSecond / 1024 / 1024:F2} MB/s";
                this.RemainingTime = _humanCalculator.GetDisplayValue(info.EstimatedTimeRemaining, DateTime.Now).ToString(@"hh\:mm\:ss");
                this.OverallProgressText = $"{info.FilesCopied} / {info.TotalFiles} files • {info.BytesCopied / 1024d / 1024d:F2} / {info.TotalBytes / 1024d / 1024d:F2} MB";
                //this.TotalCopiedText = $"{info.BytesCopied / 1024 / 1024:F2} MB of {info.TotalBytes / 1024 / 1024:F2} MB";
                //this.FilesCopiedText = $"{info.FilesCopied} / {info.TotalFiles} files"; // <-- вот здесь
                //_main.CurrentViewModel = _main.ReportVM;
            };

            PauseCommand = new RelayCommand(_ => _sessionService.Pause());
            ResumeCommand = new RelayCommand(_ => _sessionService.Resume());
            CancelCommand = new RelayCommand(_ => _sessionService.Cancel());
        }
    }
}
