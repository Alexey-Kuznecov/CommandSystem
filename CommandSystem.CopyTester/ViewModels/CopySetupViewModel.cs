using CommandSystem.Gui.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Copying;
using UnityCommander.Copying.Settings;

namespace CommandSystem.CopyTester.ViewModels
{
    public class CopySetupViewModel : ObservableObject
    {
        private string _sourcePath = "E:\\Projects\\03._Tests\\CopyFileTest\\Source3";
        private string _destinationPath = "E:\\Projects\\03._Tests\\CopyFileTest\\Target";
        private readonly MainViewModel _main;

        public CopyOptions Options { get; }

        public RelayCommand StartCopyCommand { get; }

        private readonly CopySessionService _sessionService;
        private readonly CopyManager _copyManager;

        public string SourcePath
        {
            get => _sourcePath;
            set => SetProperty(ref _sourcePath, value);
        }

        public string DestinationPath
        {
            get => _destinationPath;
            set => SetProperty(ref _destinationPath, value);
        }

        public CopySetupViewModel(CopySessionService sessionService, CopyManager copyManager, MainViewModel main)
        {
            _main = main;
            _sessionService = sessionService;
            _copyManager = copyManager;
            Options = _sessionService.Options;

            StartCopyCommand = new RelayCommand(async _ =>
            {
                await StartCopyAsync();
            });
        }

        private async Task StartCopyAsync()
        {
            _sessionService.StartSession(0, 0); // Можно потом вычислять по выбранным файлам
            _main.CurrentViewModel = _main.ProgressVM;
            await _copyManager.CopyFilesAsync(SourcePath, DestinationPath, Options, CancellationToken.None);
        }
    }
}
