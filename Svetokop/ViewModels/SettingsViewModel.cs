
using CommandSystem.Gui.MVVM;
using NLog.Filters;
using UnityCommander.Copying;
using UnityCommander.Copying.Helper;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using UnityCommander.Copying.Strategies;

namespace Svetokop.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private string _sourcePath = string.Empty;
        private string _destinationPath = string.Empty;

        private int _maxConcurrentTasks = 5;
        private int _bufferSize = 81920;
        private bool _allowEmptyDirectories;
        private bool _useMultiThreading;
        private bool _overwriteExistingFiles;
        private bool _сopyAllToOneFolder;
        private bool _flattenStructure;

        public SettingsViewModel()
        {
            SourcePath = "E:\\Projects\\03._Tests\\CopyFileTest\\Source2";
            DestinationPath = "c:\\Target";
            UseMultiThreading = false;
            MaxConcurrentTasks = 2;
        }

        public int MaxConcurrentTasks
        {
            get => _maxConcurrentTasks;
            set => SetProperty(ref _maxConcurrentTasks, value);
        }

        public int BufferSize
        {
            get => _bufferSize;
            set => SetProperty(ref _bufferSize, value);
        }

        public bool AllowEmptyDirectories
        {
            get => _allowEmptyDirectories;
            set => SetProperty(ref _allowEmptyDirectories, value);
        }

        public bool UseMultiThreading
        {
            get => _useMultiThreading;
            set => SetProperty(ref _useMultiThreading, value);
        }

        public bool OverwriteExistingFiles
        {
            get => _overwriteExistingFiles;
            set => SetProperty(ref _overwriteExistingFiles, value);
        }
        public bool FlattenStructure
        {
            get => _flattenStructure;
            set => SetProperty(ref _flattenStructure, value);
        }
        public bool CopyAllToOneFolder
        {
            get => _сopyAllToOneFolder;
            set => SetProperty(ref _сopyAllToOneFolder, value);
        }
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
    }
}
