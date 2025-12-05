
namespace CommandSystem.Testing.Executor
{
    //public class CommandExecutorTests
    //{
    //    private ICommandExecutor _commandExecutor;
    //    private Mock<ICommandHistory> _mockHistory;

    //    public CommandExecutorTests()
    //    {
    //        _mockHistory = new Mock<ICommandHistory>();
    //        _commandExecutor = new CommandExecutor(_mockHistory.Object);
    //    }

    //    /// <summary>
    //    /// Проверяет, что команда выполняется, если CanExecute возвращает true.
    //    /// </summary>
    //    [Fact]
    //    public void Execute_ShouldExecuteCommand_WhenCanExecuteReturnsTrue()
    //    {
    //        // Arrange
    //        var command = new Mock<ICommand>();
    //        var context = new CommandContext(null);
    //        command.Setup(c => c.CanExecute(context)).Returns(true);
    //        command.Setup(c => c.Execute(context)).Verifiable();

    //        // Act
    //        _commandExecutor.Execute(command.Object, context);

    //        // Assert
    //        command.Verify(c => c.Execute(context), Times.Once);
    //    }

    //    /// <summary>
    //    /// Проверяет, что команда не выполняется, если CanExecute возвращает false.
    //    /// </summary>
    //    [Fact]
    //    public void Execute_ShouldNotExecuteCommand_WhenCanExecuteReturnsFalse()
    //    {
    //        // Arrange
    //        var command = new Mock<ICommand>();
    //        var context = new CommandContext(null);
    //        command.Setup(c => c.CanExecute(context)).Returns(false);
    //        command.Setup(c => c.Execute(context)).Verifiable();

    //        // Act
    //        _commandExecutor.Execute(command.Object, context);

    //        // Assert
    //        command.Verify(c => c.Execute(context), Times.Never);
    //    }

    //    /// <summary>
    //    ///  Проверяет асинхронное выполнение команды при CanExecute = true.
    //    /// </summary>
    //    [Fact]
    //    public async Task ExecuteAsync_ShouldExecuteCommand_WhenCanExecuteReturnsTrue()
    //    {
    //        // Arrange
    //        var asyncCommand = new Mock<IAsyncCommand>();
    //        var context = new CommandContext(null);

    //        // Настроим, что команда может выполняться
    //        asyncCommand.Setup(c => c.CanExecute(context)).Returns(true);

    //        // Настроим метод ExecuteAsync, чтобы он выполнялся
    //        asyncCommand.Setup(c => c.ExecuteAsync(context)).Returns(Task.CompletedTask).Verifiable();

    //        // Act
    //        await _commandExecutor.ExecuteAsync(asyncCommand.Object, context); // Выполняем команду

    //        // Assert
    //        asyncCommand.Verify(c => c.ExecuteAsync(context), Times.Once); // Проверяем, что ExecuteAsync был вызван один раз
    //    }

    //    /// <summary>
    //    /// Проверяет, что команда не выполняется асинхронно при CanExecute = false.
    //    /// </summary>
    //    [Fact]
    //    public async Task ExecuteAsync_ShouldNotExecuteCommand_WhenCanExecuteReturnsFalse()
    //    {
    //        // Arrange
    //        var command = new Mock<IAsyncCommand>();
    //        var context = new CommandContext(null);
    //        command.Setup(c => c.CanExecute(context)).Returns(false);
    //        command.Setup(c => c.ExecuteAsync(context)).Returns(Task.CompletedTask).Verifiable();

    //        // Act
    //        await _commandExecutor.ExecuteAsync(command.Object, context);

    //        // Assert
    //        command.Verify(c => c.ExecuteAsync(context), Times.Never);
    //    }

    //    /// <summary>
    //    /// Проверяет, что команду, поддерживающую отмену, добавляют в историю.
    //    /// </summary>
    //    [Fact]
    //    public void Execute_ShouldPushToHistory_WhenCommandIsUndoable()
    //    {
    //        // Arrange
    //        var command = new Mock<IUndoableCommand>();
    //        var context = new CommandContext(null);
    //        command.Setup(c => c.CanExecute(context)).Returns(true);
    //        command.Setup(c => c.Execute(context)).Verifiable();

    //        // Act
    //        _commandExecutor.Execute(command.Object, context);

    //        // Assert
    //        _mockHistory.Verify(h => h.Push(It.Is<IUndoableCommand>(cmd => cmd == command.Object), context), Times.Once);
    //    }

    //    [Fact]
    //    public async Task ExecuteAsync_ShouldPushToHistory_WhenCommandIsUndoable()
    //    {
    //        // Arrange
    //        var undoableCommand = new Mock<IUndoableCommand>();
    //        var context = new CommandContext(null);

    //        // Настроим выполнение асинхронной команды
    //        undoableCommand.Setup(c => c.CanExecute(context)).Returns(true);

    //        // Настроим выполнение ExecuteAsync для IAsyncCommand
    //        var asyncCommand = new Mock<IAsyncCommand>();
    //        asyncCommand.Setup(c => c.ExecuteAsync(context)).Returns(Task.CompletedTask).Verifiable();

    //        // Настроим историю
    //        var mockHistory = new Mock<ICommandHistory>();
    //        _commandExecutor = new CommandExecutor(mockHistory.Object); // Передаем мок истории в CommandExecutor

    //        // Act
    //        await _commandExecutor.ExecuteAsync(asyncCommand.Object, context); // Выполняем команду

    //        // Assert
    //        mockHistory.Verify(h => h.Push(It.Is<IUndoableCommand>(cmd => cmd == undoableCommand.Object), context), Times.Once); // Проверяем, что команда с отменой добавлена в историю
    //    }

    //    /// <summary>
    //    /// Проверяет выброс ArgumentNullException, если передана null команда.
    //    /// </summary>
    //    [Fact]
    //    public void Execute_ShouldThrowArgumentNullException_WhenCommandIsNull()
    //    {
    //        // Arrange
    //        ICommand? command = null;
    //        var context = new CommandContext(null);

    //        // Act & Assert
    //        Assert.Throws<ArgumentNullException>(() => _commandExecutor.Execute(command!, context));
    //    }

    //    /// <summary>
    //    /// Проверяет выброс ArgumentNullException для асинхронной команды, если передана null команда.
    //    /// </summary>
    //    [Fact]
    //    public async Task ExecuteAsync_ShouldThrowArgumentNullException_WhenCommandIsNull()
    //    {
    //        // Arrange
    //        IAsyncCommand? command = null;
    //        var context = new CommandContext(null);

    //        // Act & Assert
    //        await Assert.ThrowsAsync<ArgumentNullException>(() => _commandExecutor.ExecuteAsync(command!, context));
    //    }
    //}
}
