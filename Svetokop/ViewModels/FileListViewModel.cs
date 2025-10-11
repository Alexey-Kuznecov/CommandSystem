using CommandSystem.Gui.MVVM;
using System.Collections.ObjectModel;
using System.Windows;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;

namespace Svetokop.ViewModels
{
    public class FileListViewModel : ObservableObject
    {
        private readonly Services.CopyFileReporter2? _fileReporter;

        // Коллекция для отображения с фильтром
        private readonly ObservableCollection<FileCopyItem> _filteredFiles = new();
        public ReadOnlyObservableCollection<FileCopyItem>? FilteredFiles { get; }

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
            // Подписка на внутреннюю коллекцию через внутренний ObservableCollection
            FilteredFiles = new ReadOnlyObservableCollection<FileCopyItem>(_filteredFiles);
            if (fileReporter is Services.CopyFileReporter2 reporter)
            {
                _fileReporter = reporter;
                reporter.FilesChanged += () => Application.Current.Dispatcher.Invoke(RefreshFilter);
            }
            RefreshFilter();
        }

        private void RefreshFilter()
        {
            _filteredFiles.Clear();

            foreach (var item in _fileReporter.Files)
            {
                if (PassesFilter(item))
                    _filteredFiles.Add(item);
            }
        }

        private bool PassesFilter(FileCopyItem item)
        {
            // Фильтр по статусу
            if (SelectedFileFilter != "Все файлы")
            {
                if (SelectedFileFilter == "В процессе" && item.Status != FileCopyStatus.InProgress) return false;
                if (SelectedFileFilter == "Скопированные" && item.Status != FileCopyStatus.Completed) return false;
                if (SelectedFileFilter == "С ошибкой" && item.Status != FileCopyStatus.Failed) return false;
            }

            // Поиск по имени или пути
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
