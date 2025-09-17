
using CommandSystem.Gui.MVVM;
using Svetokop.Services;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using UnityCommander.Copying.Reporting;

namespace Svetokop.ViewModels
{
    public class LogViewModel : ObservableObject
    {
        private readonly ICollectionView _logView;
        public ICollectionView LogView => _logView;

        private static readonly Dictionary<CopyLogType, CopyLogType[]> FilterGroups = new()
        {
            [CopyLogType.Info] = new[]
            {
                CopyLogType.Info,
                CopyLogType.FileStarted,
                CopyLogType.FileCompleted,
                CopyLogType.FileProgress
            },
            [CopyLogType.Warning] = new[] { CopyLogType.Warning },
            [CopyLogType.Error] = new[] { CopyLogType.Error, CopyLogType.Cancelled },
            [CopyLogType.Paused] = new[] { CopyLogType.Paused, CopyLogType.Resumed }
        };

        private CopyLogType? _filterLevel;
        public CopyLogType? FilterLevel
        {
            get => _filterLevel;
            set
            {
                if (SetProperty(ref _filterLevel, value))
                    _logView.Refresh(); // обновляем фильтр при изменении уровня
            }
        }

        public LogViewModel(ICopyReporter copyLog)
        {
            if (copyLog == null) throw new ArgumentNullException(nameof(copyLog));

            // используем напрямую коллекцию copyLog.Entries

            if (copyLog is CopyLogReporter logReporter)
            {
                _logView = CollectionViewSource.GetDefaultView(logReporter.Entries);
                _logView.Filter = FilterLog;
            }
        }

        private bool FilterLog(object obj)
        {
            if (obj is not CopyLogEntry entry) return false;

            if (FilterLevel == null)
                return true;

            var selected = FilterLevel.Value;

            if (!FilterGroups.TryGetValue(selected, out var group))
                // если группа не найдена — сравниваем напрямую
                if (entry.Type is CopyLogType type)
                    return type == FilterLevel;

            // нормализуем тип записи в CopyLogType (на случай, если entry.Type — boxed int)
            CopyLogType entryType = entry.Type is CopyLogType et
                ? et
                : (CopyLogType)Convert.ToInt32(entry.Type);

            return group.Contains(entryType); // требует System.Linq
        }
        private ObservableCollection<CopyLogEntry> _selectedLogEntries = new();
        public ObservableCollection<CopyLogEntry> SelectedLogEntries
        {
            get => _selectedLogEntries;
            set => SetProperty(ref _selectedLogEntries, value);
        }

        public ICommand CopyToClipboardCommand => new RelayCommand<IList>(
            items =>
            {
                if (items == null || items.Count == 0) return;
                var text = string.Join(Environment.NewLine, items.Cast<CopyLogEntry>().Select(x => x.Message));
                Clipboard.SetText(text);
            });
    }
}
