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
    public class UndoableAsyncCommandDecorator_ExecutionTests
    {
        /// <summary>
        /// Проверяет, что метод CanExecute делегирует вызов внутренней команде.
        /// </summary>
        [Fact]
        public void CanExecute_CallsInnerCommand()
        {
            // Arrange
            var commandMock = new Mock<IAsyncCommand>();
            var context = new CommandContext(null);

            commandMock.Setup(c => c.CanExecute(context)).Returns(true);
            commandMock.Setup(c => c.Name).Returns("test");
            commandMock.Setup(c => c.Description).Returns("desc");

            var decorator = new UndoableAsyncCommandDecorator(commandMock.Object);

            // Act
            var result = decorator.CanExecute(context);

            // Assert
            Assert.True(result);
            commandMock.Verify(c => c.CanExecute(context), Times.Once);
        }

        /// <summary>
        /// Проверяет, что метод ExecuteAsync делегирует выполнение внутренней команде.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_CallsInnerCommand()
        {
            // Arrange
            var commandMock = new Mock<IAsyncCommand>();
            var context = new CommandContext(null);

            commandMock.Setup(c => c.ExecuteAsync(context)).Returns(Task.CompletedTask);
            commandMock.Setup(c => c.Name).Returns("test");
            commandMock.Setup(c => c.Description).Returns("desc");

            var decorator = new UndoableAsyncCommandDecorator(commandMock.Object);

            // Act
            await decorator.ExecuteAsync(context);

            // Assert
            commandMock.Verify(c => c.ExecuteAsync(context), Times.Once);
        }
    }
}
