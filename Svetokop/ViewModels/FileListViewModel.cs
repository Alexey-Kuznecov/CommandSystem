using CommandSystem.Gui.MVVM;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;

namespace Svetokop.ViewModels
{
    public class FileListViewModel : ObservableObject
    {
        private readonly Services.CopyFileReporter? _fileReporter;
        private readonly ObservableCollection<FileCopyItem> _filteredFiles = new();
        public ReadOnlyObservableCollection<FileCopyItem> FilteredFiles { get; }

        private readonly DispatcherTimer _uiTimer;

        private string _selectedFileFilter = "Все файлы";
        public string SelectedFileFilter
        {
            get => _selectedFileFilter;
            set
            {
                if (SetProperty(ref _selectedFileFilter, value))
                    RefreshFilter();
            }
        }

        private string _fileSearchText = string.Empty;
        public string FileSearchText
        {
            get => _fileSearchText;
            set
            {
                if (SetProperty(ref _fileSearchText, value))
                    RefreshFilter();
            }
        }

        public FileListViewModel(ICopyReporter fileReporter)
        {
            FilteredFiles = new ReadOnlyObservableCollection<FileCopyItem>(_filteredFiles);

            if (fileReporter is Services.CopyFileReporter reporter)
            {
                _fileReporter = reporter;
            }

            // Таймер обновления UI
            _uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _uiTimer.Tick += (s, e) => RefreshFilter();
            _uiTimer.Start();
        }

        private void RefreshFilter()
        {
            if (_fileReporter == null) return;

            var files = _fileReporter.Files;
            if (files == null) return;

            // Пересобираем список фильтрованных файлов
            _filteredFiles.Clear();
            foreach (var item in files)
            {
                if (PassesFilter(item))
                {
                    item.UpdateDisplayValues(); // обновляем текстовые поля
                    _filteredFiles.Add(item);
                }
            }
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
                if (!(item.Source?.Contains(s, StringComparison.CurrentCultureIgnoreCase) == true ||
                      item.Destination?.Contains(s, StringComparison.CurrentCultureIgnoreCase) == true))
                    return false;
            }
            return true;
        }
    }
}
