
using CommandSystem.Gui.Core;

namespace CommandSystem.Gui.MVVM
{
    public class ReflectionCommandViewModelBinder : ICommandViewModelBinder
    {
        public void BindCommands(object viewModel)
        {
            // Находит DelegateCommand-свойства и регистрирует их, если нужно
            // Может добавлять команды в каталог
        }
    }
}
