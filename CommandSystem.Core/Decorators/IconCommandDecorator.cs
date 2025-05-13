using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Decorators
{
    public class IconCommandDecorator : ICommand
    {
        private readonly ICommand _inner;
        public string IconPath { get; }

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public IconCommandDecorator(ICommand inner, string iconPath)
        {
            _inner = inner;
            IconPath = iconPath;
        }

        public bool CanExecute(CommandContext context) => _inner.CanExecute(context);
        public void Execute(CommandContext context) => _inner.Execute(context);
    }

}
