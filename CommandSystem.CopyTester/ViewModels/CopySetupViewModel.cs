using CommandSystem.Gui.MVVM;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using UnityCommander.Copying.Strategies;

namespace CommandSystem.CopyTester.ViewModels
{
    public class CopySetupViewModel : ObservableObject
    {
        private readonly MainViewModel _main;
        private readonly CopyOptions _options;
        private string _sourcePath = string.Empty;
        private string _targetPath = string.Empty;
        public FilterOptionsViewModel FilterVM { get; } = new FilterOptionsViewModel();

        private int _maxConcurrentTasks = 5;
        public int MaxConcurrentTasks
        {
            get => _maxConcurrentTasks;
            set => SetProperty(ref _maxConcurrentTasks, value);
        }

        public bool UseMultiThreading { get; set; } = true;
        public bool OverwriteExistingFiles { get; set; } = true;
        public bool FlattenStructure { get; set; }
        public bool CopyAllToOneFolder { get; set; }

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

        public CopySetupViewModel(MainViewModel main)
        {
            SourcePath = "E:\\Projects\\03._Tests\\CopyFileTest\\Source3";
            TargetPath = "E:\\Projects\\03._Tests\\CopyFileTest\\Target";
            _main = main;

            StartCopyCommand = new RelayCommand(async _ =>
            {
                await StartCopyAsync();
            });
        }

        public async Task StartCopyAsync()
        {
            await Task.Run(() =>
            {
                var session = new CopySessionService
                {
                    SourcePath = SourcePath,
                    TargetPath = TargetPath,
                    //Options = ToCopyOptions()
                };

                _main.StartWizardCopy(session);
            });
        }

        public CopyOptions ToCopyOptions()
        {
            var settings = new CompositeCopySettings(
            [
                opts => opts.AllowEmptyDirectories = false,
                opts => opts.BufferSize = 81920,
                opts => opts.DiscoveryStrategy = new RecursiveFullDiscoveryStrategy()
            ]);

            var options = new CopyOptions
            {
                MaxConсurrentTasks = MaxConcurrentTasks,
                UseMultiThreading = UseMultiThreading,
                OverwriteExistingFiles = OverwriteExistingFiles,
                FlattenStructure = FlattenStructure,
                CopyAllToOneFolder = CopyAllToOneFolder,

                // Создаём фильтр через фабрику из VM
                FileFilter = FileFilterFactory.Create(FilterVM.ToFilterOptions())
            };

            settings.Apply(ref options);
            return options;
        }
    }
}
