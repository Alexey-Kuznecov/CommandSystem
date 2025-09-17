using CommandSystem.Gui.MVVM;
using Spectre.Console;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using UnityCommander.Copying;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;

namespace Svetokop.ViewModels
{
    public class FileListViewModel : ObservableObject
    {
        private readonly ICopyReporter _reporter;

        public ICollectionView FilesView { get; }

        private string _selectedFileFilter = "Все файлы";
        public string SelectedFileFilter
        {
            get => _selectedFileFilter;
            set
            {
                if (SetProperty(ref _selectedFileFilter, value))
                    FilesView.Refresh();
            }
        }

        private string _fileSearchText = string.Empty;
        public string FileSearchText
        {
            get => _fileSearchText;
            set
            {
                if (SetProperty(ref _fileSearchText, value))
                    FilesView.Refresh();
            }
        }

        public FileListViewModel(ICopyReporter reporter)
        {
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

            // Берём view поверх ReadOnlyObservableCollection — изменения в коллекции автоматически отражаются
            if (_reporter is CopyFileReporter fileReporter)
            {
                FilesView = CollectionViewSource.GetDefaultView(fileReporter.Files);
                FilesView.Filter = FilterPredicate;
            }
        }

        private bool FilterPredicate(object obj)
        {
            if (obj is not FileCopyItem item)
                return false;

            // фильтр по статусу
            if (SelectedFileFilter != "Все файлы")
            {
                if (SelectedFileFilter == "В процессе" && item.Status != FileCopyStatus.InProgress) return false;
                if (SelectedFileFilter == "Готово" && item.Status != FileCopyStatus.Completed) return false;
                if (SelectedFileFilter == "Ошибка" && item.Status != FileCopyStatus.Failed) return false;
            }

            // поиск по имени/пути
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
