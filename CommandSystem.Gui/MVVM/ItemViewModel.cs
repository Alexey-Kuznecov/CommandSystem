
using CommandSystem.Core.Execution;

namespace CommandSystem.Gui.MVVM
{
    public class ItemViewModel // : BindableBase
    {
        public DelegateCommand DeleteCommand { get; private set; }

        private bool _isItemSelected;
        public bool IsItemSelected
        {
            get => _isItemSelected;
            set
            {
                //SetProperty(ref _isItemSelected, value);
                //DeleteCommand.RaiseCanExecuteChanged(); // уведомляем
            }
        }

        public ItemViewModel()
        {
            //DeleteCommand = new DelegateCommand(ExecuteDelete, CanDelete);
        }

        private void ExecuteDelete()
        {
            // Логика удаления
        }

        private bool CanDelete()
        {
            return IsItemSelected;
        }
    }
}
