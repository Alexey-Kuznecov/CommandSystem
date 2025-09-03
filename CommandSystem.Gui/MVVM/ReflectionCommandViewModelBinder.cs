using CommandSystem.Gui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
