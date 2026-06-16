using CommandSystem.Gui.MVVM;
using Svetokop.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;

namespace Svetokop.ViewModels
{
    public class FileListViewModel : ObservableObject
    {
        private readonly CopyFileReporter _fileReporter;
        private readonly ObservableCollection<FileCopyItem> _filteredFiles = new();
        public ObservableCollection<FileCopyItem> FilteredFiles { get; set; }

        private string _selectedFileFilter = "Все файлы";
        public string SelectedFileFilter { get => _selectedFileFilter; set { SetProperty(ref _selectedFileFilter, value); RefreshFilter(); } }

        private string _fileSearchText = string.Empty;
        public string FileSearchText { get => _fileSearchText; set { SetProperty(ref _fileSearchText, value); RefreshFilter(); } }

        private bool _pendingRefresh = false;
        private readonly DispatcherTimer _refreshTimer;

        public FileListViewModel(CopyFileReporter reporter)
        {
            //_fileReporter = reporter;
            //_fileReporter.FilesChanged += () => _pendingRefresh = true;

            //FilteredFiles = new ObservableCollection<FileCopyItem>(_filteredFiles);

            //_refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1) };
            //_refreshTimer.Tick += (s, e) => RefreshFilter();
            //_refreshTimer.Start();
        }

        private void RefreshFilter()
        {
            //if (!_pendingRefresh) return;
            //_pendingRefresh = false;

            //_filteredFiles.Clear();

            //FilteredFiles = _fileReporter.Files;
            //foreach (var item in _fileReporter.Files)
            //{
            //    if (PassesFilter(item)) _filteredFiles.Add(item);
            //}
        }

        private bool PassesFilter(FileCopyItem item)
        {
            if (SelectedFileFilter != "Все файлы")
            {
                if (SelectedFileFilter == "В процессе" && item.Status != FileCopyStatus.InProgress) return false;
                if (SelectedFileFilter == "Скопированные" && item.Status != FileCopyStatus.Completed) return false;
                if (SelectedFileFilter == "С ошибкой" && item.Status != FileCopyStatus.Failed) return false;
            }

            if (!string.IsNullOrWhiteSpace(FileSearchText))
            {
                var s = FileSearchText.Trim();
                if (!(item.Source.Contains(s, StringComparison.CurrentCultureIgnoreCase) ||
                      item.Destination.Contains(s, StringComparison.CurrentCultureIgnoreCase)))
                    return false;
            }
            return true;
        }
    }

}
