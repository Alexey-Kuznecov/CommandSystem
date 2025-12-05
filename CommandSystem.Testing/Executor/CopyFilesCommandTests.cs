
namespace CommandSystem.Testing.Executor;

public class CopyFilesCommandTests
{
    //private IFileSystem _fileSystem;
    //private CommandExecutor _executor;

    //public CopyFilesCommandTests()
    //{
    //    _fileSystem = new MockFileSystem(); // Это может быть mock-реализация файловой системы
    //    _executor = new CommandExecutor();  // Если CommandExecutor уже реализован
    //}

    //[Fact]
    //public async Task CopyFiles_ShouldCopyFile_WhenCanExecuteReturnsTrue()
    //{
    //    // Arrange
    //    var sourcePath = "source.txt";
    //    var destinationPath = "destination.txt";
    //    _fileSystem.CreateFile(sourcePath, "Content of source file");

    //    var command = new CopyFilesCommand(sourcePath, destinationPath, _fileSystem);

    //    // Act
    //    await _executor.ExecuteAsync(command, new CommandContext(null));

    //    // Assert
    //    Assert.True(_fileSystem.Exists(destinationPath));  // Убедимся, что файл был скопирован
    //}

    //[Fact]
    //public async Task CopyFiles_ShouldNotCopyFile_WhenSourceDoesNotExist()
    //{
    //    // Arrange
    //    var sourcePath = "nonexistent.txt";
    //    var destinationPath = "destination.txt";
    //    var command = new CopyFilesCommand(sourcePath, destinationPath, _fileSystem);

    //    // Act
    //    await _executor.ExecuteAsync(command, new CommandContext(null));

    //    // Assert
    //    Assert.False(_fileSystem.Exists(destinationPath));  // Убедимся, что файл не был скопирован
    //}

    //[Fact]
    //public async Task CopyFiles_ShouldSupportUndo_WhenCommandIsUndoable()
    //{
    //    // Arrange
    //    var sourcePath = "source.txt";
    //    var destinationPath = "destination.txt";
    //    _fileSystem.CreateFile(sourcePath, "Content of source file");

    //    var command = new CopyFilesCommand(sourcePath, destinationPath, _fileSystem);
    //    _executor.ExecuteAsync(command, new CommandContext(null));

    //    // Убираем файл обратно при отмене
    //    _fileSystem.DeleteFile(destinationPath);

    //    // Act
    //    // Тут можно протестировать отмену с помощью истории команд, если такова есть.

    //    Assert.False(_fileSystem.Exists(destinationPath));  // Убедимся, что файл не был скопирован после отмены
    //}

    //[Fact]
    //public async Task CopyFilesCommand_ShouldUndoSuccessfully()
    //{
    //    // Arrange
    //    var sourcePath = "source.txt";
    //    var destinationPath = "destination.txt";
    //    var context = new CommandContext(null);
    //    _fileSystem.CreateFile(sourcePath, "Some content");

    //    var command = new CopyFilesCommand(sourcePath, destinationPath, _fileSystem);

    //    // Act — выполняем команду
    //    await command.ExecuteAsync(context);

    //    Assert.True(_fileSystem.Exists(destinationPath)); // файл должен быть скопирован

    //    // Act — откатываем
    //    await command.UndoAsync(context);

    //    // Assert — проверяем, что файл удалён
    //    Assert.False(_fileSystem.Exists(destinationPath));
    //}

    //[Fact(Timeout = 10000)]
    //public async Task CopyFilesCommand_Should_Respect_Cancellation()
    //{
    //    //// Arrange
    //    //var sourcePath = "source.txt";
    //    //var destinationPath = "destination.txt";
    //    //_fileSystem.CreateFile(sourcePath, new string('A', 100)); // большой контент

    //    //var command = new CopyFilesCommand(sourcePath, destinationPath, _fileSystem);

    //    //var cts = new CancellationTokenSource();
    //    //var context = new CommandContext(cts.Token);

    //    //// Отменяем токен ДО начала копирования
    //    //cts.Cancel();

    //    //// Act - начинаем выполнение команды
    //    ////var executingTask = _executor.ExecuteAsync(context);

    //    //// Assert - ожидаем, что выполнение приведет к исключению отмены
    //    //await Assert.ThrowsAsync<TaskCanceledException>(() => executingTask);

    //    //// Assert - файл не должен быть скопирован
    //    //Assert.False(_fileSystem.Exists(destinationPath));
    //}
}