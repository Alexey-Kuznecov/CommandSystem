
using System.Diagnostics.CodeAnalysis;
using CommandSystem.Console.Core;

namespace CommandSystem.Console.Commands
{
    /// <summary>
    /// Реализация интерфейса <see cref="ICommandParameters"/> для хранения и работы с параметрами консольных команд.
    /// Позволяет задавать и получать значения по имени параметра с возможностью преобразования типов.
    /// </summary>
    public class ConsoleCommandParameters : ICommandParameters
    {
        /// <summary>
        /// Внутреннее хранилище параметров команд.
        /// </summary>
        private readonly Dictionary<string, object?> _parameters = new();

        /// <summary>
        /// Устанавливает значение параметра по имени.
        /// </summary>
        /// <typeparam name="T">Тип значения.</typeparam>
        /// <param name="name">Имя параметра.</param>
        /// <param name="value">Значение параметра.</param>
        public void Set<T>(string name, T value) => Set(name, (object?)value);

        /// <summary>
        /// Получает значение параметра по имени с приведением к указанному типу.
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип значения.</typeparam>
        /// <param name="name">Имя параметра.</param>
        /// <returns>
        /// Значение параметра приведённое к типу <typeparamref name="T"/>, либо <c>default</c>, если параметр не найден или приведение не удалось.
        /// </returns>
        [return: MaybeNull]
        public T? Get<T>(string name)
        {
            if (!_parameters.TryGetValue(name, out var value))
                return default;

            try
            {
                if (value is T t) return t;
                return (T?)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Устанавливает значение параметра по ключу.
        /// </summary>
        /// <param name="key">Имя параметра.</param>
        /// <param name="value">Значение параметра (может быть <c>null</c>).</param>
        public void Set([DisallowNull] string key, object? value)
        {
            _parameters[key] = value;
        }

        /// <summary>
        /// Проверяет, содержит ли коллекция параметр с указанным именем.
        /// </summary>
        /// <param name="key">Имя параметра.</param>
        /// <returns><c>true</c>, если параметр с таким именем существует; иначе <c>false</c>.</returns>
        public bool Contains([NotNullWhen(true)] string key) => _parameters.ContainsKey(key);
    }
}

