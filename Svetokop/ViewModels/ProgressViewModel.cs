
using CommandSystem.Gui.MVVM;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Input;
using UnityCommander.Copying;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Sessions;

namespace Svetokop.ViewModels
{
    public class ProgressViewModel : ObservableObject, IDisposable
    {
        private readonly CopyManager _copyManager;
        private readonly CopySessionManager _sessionManager;
        private readonly HumanReadableTimeCalculator _humanCalculator = new();

        private int _fileProgress;
        private string _fileProgressText = string.Empty;
        private int _totalProgress;
        private string _totalProgressText = string.Empty;
        private string _currentFileName = string.Empty;
        private string _currentFilePath = string.Empty;
        private string _remainingTime = string.Empty;
        private string _totalCopiedText = string.Empty;
        private string _speedText = string.Empty;
        private string _filesCopiedText = string.Empty;
        private IDisposable? _subscription;

        public event Func<Task>? StartRequested = null;

        public ProgressViewModel(CopyManager copyManager, CopySessionManager copySessionManager)
        {
            _copyManager = copyManager ?? throw new ArgumentNullException(nameof(copyManager));
            _sessionManager = copySessionManager ?? throw new ArgumentNullException(nameof(copySessionManager));

            StartCommand = new RelayCommand(async _ => await StartRequested.Invoke(), _ => State == SessionState.Idle);
            PauseCommand = new RelayCommand(_ => _sessionManager.CurrentSession?.Pause(), _ => State == SessionState.Running);
            ResumeCommand = new RelayCommand(_ => _sessionManager.CurrentSession?.Resume(), _ => State == SessionState.Paused);
            CancelCommand = new RelayCommand(_ => _sessionManager.CurrentSession?.Cancel(), _ => State == SessionState.Running || State == SessionState.Paused);

            // Подписка на поток прогресса
            SubscribeToProgress();

            // Подписка на изменение состояния сессии
            _sessionManager.CurrentSessionStateChanged += (s, state) => State = state;
        }

        #region Dependency Properties

        public string CurrentFilePath
        {
            get => _currentFilePath;
            set => SetProperty(ref _currentFilePath, value);
        }

        public string CurrentFileName
        {
            get => _currentFileName;
            set => SetProperty(ref _currentFileName, value);
        }

        public int FileProgress
        {
            get => _fileProgress;
            set => SetProperty(ref _fileProgress, value);
        }

        public int TotalProgress
        {
            get => _totalProgress;
            set => SetProperty(ref _totalProgress, value);
        }

        public string FileProgressText
        {
            get => _fileProgressText;
            set => SetProperty(ref _fileProgressText, value);
        }

        public string TotalProgressText
        {
            get => _totalProgressText;
            set => SetProperty(ref _totalProgressText, value);
        }

        public string TimeRemaining
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        public string CurrentSpeed
        {
            get => _speedText;
            set => SetProperty(ref _speedText, value);
        }

        public string FilesCopiedText
        {
            get => _filesCopiedText;
            set => SetProperty(ref _filesCopiedText, value);
        }

        public string TotalCopiedText
        {
            get => _totalCopiedText;
            set => SetProperty(ref _totalCopiedText, value);
        }

        #endregion

        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand CancelCommand { get; }

        private SessionState _state = SessionState.Idle;
        public SessionState State
        {
            get => _state;
            set
            {
                if (SetProperty(ref _state, value))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        private void RaiseCanExecuteChanged()
        {
            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PauseCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ResumeCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
        }

        private void UpdateProgress(ProgressInfo info)
        {
            CurrentFilePath = info.CurrentFilePath ?? string.Empty;
            CurrentFileName = !string.IsNullOrEmpty(info.CurrentFilePath)
                ? Path.GetFileName(info.CurrentFilePath)
                : string.Empty;

            FileProgress = info.CurrentFilePercentage;
            FileProgressText = info.CurrentFileProgressText;
            TotalProgress = (int)Math.Round(info.CompletionPercentage);
            TotalProgressText = info.TotalProgressText;

            CurrentSpeed = $"{info.SpeedBytesPerSecond / 1024.0 / 1024.0:F2} MB/s";
            TimeRemaining = _humanCalculator.GetDisplayValue(info.EstimatedTimeRemaining, DateTime.Now)
                .ToString(@"hh\:mm\:ss");
            FilesCopiedText = $"{info.FilesCopied} / {info.TotalFiles} files";
            TotalCopiedText = $"{info.BytesCopied / 1024.0 / 1024.0:F2} MB of {info.TotalBytes / 1024.0 / 1024.0:F2} MB";
        }

        private void SubscribeToProgress()
        {
            _subscription = _copyManager.ProgressStream
                .Throttle(TimeSpan.FromMilliseconds(1))
                .ObserveOn(SynchronizationContext.Current!)
                .Subscribe(UpdateProgress);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
