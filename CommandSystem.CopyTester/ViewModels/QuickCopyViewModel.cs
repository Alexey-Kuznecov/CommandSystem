using CommandSystem.Gui.MVVM;

namespace CommandSystem.CopyTester.ViewModels
{
    public class QuickCopyViewModel : ObservableObject
    {
        private readonly MainViewModel _main;
        private string _sourcePath = string.Empty;
        private string _targetPath = string.Empty;

        public string SourcePath
        {
            get => _sourcePath;
            set => SetProperty(ref _sourcePath, value);
        }

        public string TargetPath
        {
            get => _targetPath;
            set => SetProperty(ref _targetPath, value);
        }

        public RelayCommand StartCopyCommand { get; }

        public QuickCopyViewModel(MainViewModel main)
        {
            _main = main;
            StartCopyCommand = new RelayCommand(_ => StartCopy());
        }

        private void StartCopy()
        {
            _main.StartQuickCopy(SourcePath, TargetPath);
        }
    }
}
