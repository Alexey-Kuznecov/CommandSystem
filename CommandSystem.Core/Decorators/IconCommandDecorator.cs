
using CommandSystem.Abstractions;

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
