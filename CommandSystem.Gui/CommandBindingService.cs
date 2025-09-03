using CommandSystem.Gui.Core;
using CommandSystem.Gui.MVVM;

namespace CommandSystem.Gui
{
    public class CommandBindingService
    {
        private readonly ICommandViewModelBinder _binder;
        private readonly ICommandCatalog _catalog;
        private readonly CommandAvailabilityNotifier _notifier;

        public CommandBindingService(
            ICommandViewModelBinder binder,
            ICommandCatalog catalog,
            CommandAvailabilityNotifier notifier)
        {
            _binder = binder;
            _catalog = catalog;
            _notifier = notifier;
        }

        public void Bind(object viewModel)
        {
            _binder.BindCommands(viewModel);

            if (viewModel is ItemViewModel itemVM)
            {
                _catalog.RegisterCommand("delete", itemVM.DeleteCommand, viewModel);

                // Пример подписки на уведомление
                //_notifier.AvailabilityChanged += () =>
                //    itemVM.DeleteCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
