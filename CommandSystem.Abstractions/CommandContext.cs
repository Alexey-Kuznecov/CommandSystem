
using Microsoft.Extensions.DependencyInjection;

namespace CommandSystem.Abstractions
{
    public class CommandContext
    {
        private readonly Dictionary<string, object> _data = new();
        public string? Name { get; init; }

        public object? Parameter { get; init; }

        public object? Context { get; init; }

        public IServiceProvider? Services { get; init; }

        public CancellationToken CancellationToken { get; init; }
        
        public object? Result { get; set; }

        public bool SuppressHistory { get; set; }

        public CommandContext(
            string? name, 
            IServiceProvider services, 
            object? parameter = null, 
            object? context = null, 
            CancellationToken cancellationToken = default)
        {
            Name = name;
            Services = services;
            Parameter = parameter;
            CancellationToken = cancellationToken;
            Context = context;
        }

        public CommandContext(
            string? name = null,
            object? context = null,
            object? parameter = null,
            CancellationToken cancellationToken = default)
        {
            Name = name;
            Parameter = parameter;
            CancellationToken = cancellationToken;
            Context = context;
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

        public void ThrowIfCancellationRequested()
        {
            CancellationToken.ThrowIfCancellationRequested();
        }
    }
}
