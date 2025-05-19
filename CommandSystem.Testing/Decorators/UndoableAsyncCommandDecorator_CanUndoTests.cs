using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Decorators;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Testing.Decorators
{
    //public class UndoableAsyncCommandDecorator_CanUndoTests
    //{
    //    /// <summary>
    //    /// Проверяет, что метод CanUndo возвращает значение, заданное в canUndoHandler.
    //    /// </summary>
    //    [Theory]
    //    [InlineData(true)]
    //    [InlineData(false)]
    //    public void CanUndo_ReturnsExpectedValue(bool expected)
    //    {
    //        // Arrange: создаем заглушку команды и оборачиваем её в декоратор с canUndoHandler
    //        var commandMock = new Mock<IAsyncCommand>();
    //        commandMock.Setup(c => c.Name).Returns("test");
    //        commandMock.Setup(c => c.Description).Returns("desc");

    //        var decorator = new UndoableAsyncCommandDecorator(
    //            commandMock.Object,
    //            undoHandler: null,
    //            canUndoHandler: _ => expected
    //        );

    //        var context = new CommandContext(null);

    //        // Act
    //        var result = decorator.CanUndo(context);

    //        // Assert
    //        Assert.Equal(expected, result);
    //    }

    //    /// <summary>
    //    /// Проверяет, что метод CanUndo возвращает false, если canUndoHandler не задан.
    //    /// </summary>
    //    [Fact]
    //    public void CanUndo_ReturnsFalse_WhenHandlerIsNull()
    //    {
    //        // Arrange: создаем декоратор без обработчика canUndoHandler
    //        var commandMock = new Mock<IAsyncCommand>();
    //        commandMock.Setup(c => c.Name).Returns("test");
    //        commandMock.Setup(c => c.Description).Returns("desc");

    //        var decorator = new UndoableAsyncCommandDecorator(
    //            commandMock.Object,
    //            undoHandler: null,
    //            canUndoHandler: null
    //        );

    //        var context = new CommandContext(null);

    //        // Act
    //        var result = decorator.CanUndo(context);

    //        // Assert
    //        Assert.False(result);
    //    }

    //    /// <summary>
    //    /// Проверяет, что CanUndo возвращает true, если обработчик CanUndo возвращает true.
    //    /// </summary>
    //    [Fact]
    //    public void CanUndo_HandlerReturnsTrue_ReturnsTrue()
    //    {
    //        // Arrange
    //        var commandMock = new Mock<IAsyncCommand>();
    //        commandMock.Setup(c => c.Name).Returns("test");
    //        commandMock.Setup(c => c.Description).Returns("desc");

    //        var context = new CommandContext(null);
    //        var decorator = new UndoableAsyncCommandDecorator(
    //            commandMock.Object,
    //            undoHandler: null,
    //            canUndoHandler: ctx => true);

    //        // Act
    //        var result = decorator.CanUndo(context);

    //        // Assert
    //        Assert.True(result);
    //    }

    //    /// <summary>
    //    /// Проверяет, что CanUndo возвращает false, если обработчик CanUndo возвращает false.
    //    /// </summary>
    //    [Fact]
    //    public void CanUndo_HandlerReturnsFalse_ReturnsFalse()
    //    {
    //        // Arrange
    //        var commandMock = new Mock<IAsyncCommand>();
    //        commandMock.Setup(c => c.Name).Returns("test");
    //        commandMock.Setup(c => c.Description).Returns("desc");

    //        var context = new CommandContext(null);
    //        var decorator = new UndoableAsyncCommandDecorator(
    //            commandMock.Object,
    //            undoHandler: null,
    //            canUndoHandler: ctx => false);

    //        // Act
    //        var result = decorator.CanUndo(context);

    //        // Assert
    //        Assert.False(result);
    //    }

    //    /// <summary>
    //    /// Проверяет, что CanUndo возвращает false, если обработчик CanUndo отсутствует.
    //    /// </summary>
    //    [Fact]
    //    public void CanUndo_NoHandler_ReturnsFalse()
    //    {
    //        // Arrange
    //        var commandMock = new Mock<IAsyncCommand>();
    //        commandMock.Setup(c => c.Name).Returns("test");
    //        commandMock.Setup(c => c.Description).Returns("desc");

    //        var context = new CommandContext(null);
    //        var decorator = new UndoableAsyncCommandDecorator(commandMock.Object);

    //        // Act
    //        var result = decorator.CanUndo(context);

    //        // Assert
    //        Assert.False(result);
    //    }
    //}
}
