
using CommandSystem.Abstractions;
using CommandSystem.Abstractions.Metadata;

namespace CommandSystem.Core.Decorators
{
    public class CommandMetadataDecorator : IAsyncCommand, IHasIcon, IHasHotkey
    {
        private readonly IAsyncCommand _inner;

        public string? IconPath { get; }
        public string? HotkeyGesture { get; }

        public string Name => _inner.Name;
        public string Description => _inner.Description;

        public CommandMetadataDecorator(
            IAsyncCommand inner,
            string? iconPath = null,
            string? hotkeyGesture = null)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            IconPath = iconPath;
            HotkeyGesture = hotkeyGesture;
        }

        public bool CanExecute(CommandContext context) => _inner.CanExecute(context);

        public Task<UndoToken?> ExecuteAsync(CommandContext context) => _inner.ExecuteAsync(context);

        public Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken) => _inner.ExecuteAsync(context, cancellationToken);
    }
}

