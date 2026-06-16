using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Abstractions
{
    public interface IGuiCommandExecutor
    {
        /// <summary>
        /// Выполняет команду синхронно.
        /// </summary>
        CommandContext Execute(
            string commandName,
            object? parameter = null,
            CommandContext? context = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполняет команду асинхронно.
        /// </summary>
        Task ExecuteAsync(
            string commandName,
            object? parameter = null,
            CommandContext? context = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполняет команду с конкретным параметром и метаданными.
        /// (Используется, когда команда типизирована)
        /// </summary>
        Task ExecuteAsync<T>(
            CommandMetadata metadata,
            T parameter,
            CommandContext context,
            CancellationToken cancellationToken = default);
    }
}
