using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;

namespace CommandSystem.Testing.Executor.Mock
{
    public class CopyFilesCommand : IUndoableAsyncCommand
    {
        private readonly string _sourcePath;
        private readonly string _destinationPath;
        private readonly IFileSystem _fileSystem;

        public CopyFilesCommand(string sourcePath, string destinationPath, IFileSystem fileSystem)
        {
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
            _fileSystem = fileSystem;
        }

        public string Name => "CopyFiles";
        public string Description => "Copies files from source to destination";

        public bool CanExecute(CommandContext context) => _fileSystem.Exists(_sourcePath);

        public async Task ExecuteAsync(CommandContext context)
        {
            if (CanExecute(context))
            {
                await _fileSystem.CopyAsync(_sourcePath, _destinationPath, context.CancellationToken);
            }
        }

        public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (CanExecute(context))
            {
                await _fileSystem.CopyAsync(_sourcePath, _destinationPath, cancellationToken);
            }
        }

        public Task UndoAsync(CommandContext context)
        {
            if (_fileSystem.Exists(_destinationPath))
            {
                _fileSystem.DeleteFile(_destinationPath);
            }
            return Task.CompletedTask;
        }
    }
}
