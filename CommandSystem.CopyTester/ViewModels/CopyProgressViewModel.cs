
using CommandSystem.Gui.MVVM;
using System.Windows;
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
        private string _statusText = string.Empty; // Initialize to avoid nullability warning
        public int Percentage
        {
            get => _percentage;
            private set => SetProperty(ref _percentage, value);
        }

        public string StatusText
        {
            get => _statusText;
            private set => SetProperty(ref _statusText, value);
        }

        public RelayCommand PauseCommand { get; }
        public RelayCommand ResumeCommand { get; }
        public RelayCommand CancelCommand { get; }

        public CopyProgressViewModel(CopySessionService sessionService, IProgressReporter reporter, MainViewModel main)
        {
            _sessionService = sessionService;
            _main = main;
            reporter.ProgressChanged += info =>
            {
              
                int progress = (int)Math.Round(info.CompletionPercentage);
                Percentage = progress;

                var progressBar = new string('#', Math.Min(20, progress / 5));
                var remaining = new string('-', 20 - progress / 5);

                var formattedBytes = $"{info.BytesCopied / 1024 / 1024:F2} MB";
                var speedFormatted = $"{info.SpeedBytesPerSecond / 1024 / 1024:F2} MB/s";
                var readable = _humanCalculator.GetDisplayValue(info.EstimatedTimeRemaining, DateTime.UtcNow);

                StatusText = $"{progress,3}% " +
                                $"({info.FilesCopied}/{info.TotalFiles}), {formattedBytes.PadLeft(5)} " +
                                $"Speed: {speedFormatted.PadLeft(5)} ETA: {readable:mm\\:ss}";
            
            };
            
            _main.CurrentViewModel = _main.ReportVM;

            PauseCommand = new RelayCommand(_ => _sessionService.Pause());
            ResumeCommand = new RelayCommand(_ => _sessionService.Resume());
            CancelCommand = new RelayCommand(_ => _sessionService.Cancel());
        }
    }
}
