
using CommandSystem.Abstractions;

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
