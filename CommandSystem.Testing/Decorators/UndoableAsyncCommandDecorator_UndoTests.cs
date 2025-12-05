
namespace CommandSystem.Testing.Decorators
{
    using System;
    using System.Threading.Tasks;
    using global::CommandSystem.Core.Commands;
    using global::CommandSystem.Core.Decorators;
    using Moq;
    using Xunit;

    namespace CommandSystem.Tests.Decorators
    {
        //public class UndoableAsyncCommandDecorator_UndoTests
        //{
        //    /// <summary>
        //    /// Проверяет, что Undo вызывает переданный UndoHandler.
        //    /// </summary>
        //    [Fact]
        //    public async Task Undo_CallsUndoHandler()
        //    {
        //        // Arrange
        //        var commandMock = new Mock<IAsyncCommand>();
        //        commandMock.Setup(c => c.Name).Returns("test");
        //        commandMock.Setup(c => c.Description).Returns("desc");

        //        var context = new CommandContext(null);
        //        bool undoCalled = false;

        //        Func<CommandContext, Task> undoHandler = ctx =>
        //        {
        //            undoCalled = true;
        //            return Task.CompletedTask;
        //        };

        //        var decorator = new UndoableAsyncCommandDecorator(commandMock.Object, undoHandler);

        //        // Act
        //        decorator.UndoAsync(context);

        //        // Поскольку Undo вызывает UndoAsync без await, даем ему время выполниться
        //        await Task.Delay(100);

        //        // Assert
        //        Assert.True(undoCalled);
        //    }

        //    /// <summary>
        //    /// Проверяет, что Undo не вызывает исключений, если UndoHandler не задан.
        //    /// </summary>
        //    [Fact]
        //    public async Task Undo_WithoutHandler_DoesNothing()
        //    {
        //        // Arrange
        //        var commandMock = new Mock<IAsyncCommand>();
        //        commandMock.Setup(c => c.Name).Returns("test");
        //        commandMock.Setup(c => c.Description).Returns("desc");

        //        var context = new CommandContext(null);
        //        var decorator = new UndoableAsyncCommandDecorator(commandMock.Object);

        //        // Act
        //        var exception = await Record.ExceptionAsync(async () =>
        //        {
        //            decorator.UndoAsync(context);
        //            await Task.Delay(50);
        //        });

        //        // Assert
        //        Assert.Null(exception);
        //    }
        //}
    }
}
