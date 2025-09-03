using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CommandSystem.Core.Commands
{
    public class CommandContext
    {
        public string? Name { get; }
        public object? Parameter { get; }
        public IServiceProvider? Services { get; }
        public CancellationToken CancellationToken { get; }

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
