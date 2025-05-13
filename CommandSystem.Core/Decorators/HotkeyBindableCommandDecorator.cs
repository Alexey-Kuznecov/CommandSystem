using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Core.Decorators
{
    public class HotkeyBindableCommandDecorator : ICommand
    {
        private readonly ICommand _inner;
        public string Hotkey { get; }

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public HotkeyBindableCommandDecorator(ICommand inner, string hotkey)
        {
            _inner = inner;
            Hotkey = hotkey;
        }

        public bool CanExecute(CommandContext context) => _inner.CanExecute(context);
        public void Execute(CommandContext context) => _inner.Execute(context);
    }
}
