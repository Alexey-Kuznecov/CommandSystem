
using Microsoft.Extensions.DependencyInjection;

namespace CommandSystem.Abstractions
{
    public class CommandContext
    {
        private readonly Dictionary<string, object> _data = new();
        public string? Name { get; }
        public object? Parameter { get; }
        public IServiceProvider? Services { get; }
        public CancellationToken CancellationToken { get; }
        public object? Result { get; set; }
        public bool SuppressHistory { get; set; }

        public CommandContext(string? name, IServiceProvider services, object? parameter = null, CancellationToken cancellationToken = default)
        {
            Name = name;
            Services = services;
            Parameter = parameter;
            CancellationToken = cancellationToken;
        }

        public CommandContext(string? name = null, object? parameter = null, CancellationToken cancellationToken = default)
        {
            Name = name;
            Parameter = parameter;
            CancellationToken = cancellationToken;
        }

        public void Set<T>(T value)
        {
            
        }

        public T? Get<T>(string key, string dd)
        {
            return _data.TryGetValue(key, out var val)
                ? (T)val
                : default;
        }

        public T GetService<T>() where T : notnull
        {
            if (Services != null)
                return (T)Services.GetRequiredService(typeof(T));
            return default!;
        }

        /// <summary>
        /// Бросает исключение, если токен отменён.
        /// Удобно вызывать в командах.
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            CancellationToken.ThrowIfCancellationRequested();
        }
    }
}
