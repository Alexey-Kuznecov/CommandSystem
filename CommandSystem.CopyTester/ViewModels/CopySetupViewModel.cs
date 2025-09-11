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
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Settings;

namespace CommandSystem.CopyTester.ViewModels
{
    public class CopySetupViewModel : ObservableObject
    {
        private readonly MainViewModel _main;
        private readonly CopyOptions _options;
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

        public CopySetupViewModel(MainViewModel main)
        {
            SourcePath = "E:\\Projects\\03._Tests\\CopyFileTest\\Source3";
            TargetPath = "E:\\Projects\\03._Tests\\CopyFileTest\\Target";
            _main = main;
            _options = new CopyOptions();

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
                    Options = _options
                };

                _main.StartWizardCopy(session);
            });
        }
    }
}
