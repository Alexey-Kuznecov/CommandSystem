
namespace CommandSystem.Testing.Decorators
{
    //public class UndoableAsyncCommandDecoratorTests
    //{
    //    [Fact]
    //    public void Undo_CallsUndoHandler_WhenProvided()
    //    {
    //        // Arrange
    //        var undoCalled = false;
    //        var commandMock = new Mock<IAsyncCommand>();

    //        commandMock.Setup(c => c.Name).Returns("test");
    //        commandMock.Setup(c => c.Description).Returns("desc");
    //        commandMock.Setup(c => c.CanExecute(It.IsAny<CommandContext>())).Returns(true);
    //        commandMock.Setup(c => c.ExecuteAsync(It.IsAny<CommandContext>()))
    //            .Returns(Task.CompletedTask);

    //        var decorator = new UndoableAsyncCommandDecorator(
    //            commandMock.Object,
    //            undoHandler: async _ =>
    //            {
    //                await Task.Delay(10); // имитируем async
    //                undoCalled = true;
    //            },
    //            canUndoHandler: _ => true
    //        );

    //        var context = new CommandContext(null);

    //        // Act
    //        decorator.UndoAsync(context);

    //        // ждем немного, чтобы async UndoAsync успел выполниться
    //        Task.Delay(50).Wait();

    //        // Assert
    //        Assert.True(undoCalled);
    //    }

    //    [Fact]
    //    public async Task Undo_ShouldNotThrow_WhenUndoHandlerIsNull()
    //    {
    //        // Arrange
    //        var command = new UndoableAsyncCommandDecorator<string>(
    //            new AsyncDelegateCommand<string>("Test", "Test", async (param, context) => await Task.CompletedTask),
    //            undoHandler: null
    //        );

    //        // Act & Assert
    //        var exception = await Record.ExceptionAsync(() => command.UndoAsync(new CommandContext(null!))); // Используем Record.ExceptionAsync для тестирования async кода
    //        Assert.Null(exception);  // Убедимся, что исключений нет
    //    }

    //    [Fact]
    //    public void CanUndo_ShouldReturnFalse_WhenCanUndoHandlerIsNull()
    //    {
    //        // Arrange
    //        var command = new UndoableAsyncCommandDecorator<string>(
    //            new AsyncDelegateCommand<string>("Test", "Test", async (param, context) => await Task.CompletedTask),
    //            undoHandler: async (context) => await Task.CompletedTask,  // устанавливаем обработчик undo
    //            canUndoHandler: null  // canUndoHandler равен null
    //        );

    //        // Act
    //        var result = command.CanUndo(new CommandContext(null));

    //        // Assert
    //        Assert.False(result);  // Ожидаем, что CanUndo вернёт false
    //    }

    //    [Fact]
    //    public void Constructor_ShouldThrowArgumentNullException_WhenInnerCommandIsNull()
    //    {
    //        // Arrange & Act & Assert
    //        var exception = Assert.Throws<ArgumentNullException>(() =>
    //            new UndoableAsyncCommandDecorator<string>(null, undoHandler: _ => Task.CompletedTask)
    //        );

    //        Assert.Equal("inner", exception.ParamName);  // Ожидаем, что исключение выбросится с правильным параметром
    //    }

    //    [Fact]
    //    public void Execute_ShouldThrowNotImplementedException_WhenUsingInvalidType()
    //    {
    //        // Arrange
    //        var command = new UndoableAsyncCommandDecorator<string>(
    //            new AsyncDelegateCommand<string>(
    //                "Test",
    //                "Test",
    //                async (param, context) => await Task.CompletedTask // Асинхронная лямбда, возвращающая Task
    //            ),
    //            undoHandler: async (context) => await Task.CompletedTask // Асинхронный обработчик Undo
    //        );

    //        //// Act & Assert
    //        //var exception = Assert.Throws<NotImplementedException>(() => command.ExecuteAsync(new CommandContext(null)));
    //        //Assert.Equal("The method or operation is not implemented.", exception.Message);
    //    }

    //    [Fact]
    //    public void CanExecute_ShouldThrowNotImplementedException_WhenPassingTAndContext()
    //    {
    //        // Arrange
    //        var command = new UndoableAsyncCommandDecorator<string>(
    //            new AsyncDelegateCommand<string>(
    //                "Test",
    //                "Test",
    //                async (param, context) => await Task.CompletedTask // Асинхронная лямбда
    //            ),
    //            undoHandler: async (context) => await Task.CompletedTask
    //        );

    //        // Act & Assert
    //        var exception = Assert.Throws<NotImplementedException>(() => command.CanExecute("test", new CommandContext(null)));
    //        Assert.Equal("The method or operation is not implemented.", exception.Message);
    //    }
    //}
}