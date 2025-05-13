using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Abstractions.Metadata;
using CommandSystem.Core.Commands;

namespace CommandSystem.Core.Decorators
{
    public class CommandMetadataDecorator<T> : IAsyncCommand<T>, IHasIcon, IHasHotkey
    {
        private readonly IAsyncCommand<T> _inner;

        public string? IconPath { get; }
        public string? HotkeyGesture { get; }

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public CommandMetadataDecorator(
            IAsyncCommand<T> inner,
            string? iconPath = null,
            string? hotkeyGesture = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            IconPath = iconPath;
            HotkeyGesture = hotkeyGesture;
        }

        public bool CanExecute(T parameter, CommandContext context) => _inner.CanExecute(parameter, context);

        public Task ExecuteAsync(T parameter, CommandContext context, CancellationToken cancellationToken) =>
            _inner.ExecuteAsync(parameter, context, cancellationToken);
    }
}