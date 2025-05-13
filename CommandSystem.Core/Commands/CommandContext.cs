using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CommandSystem.Core.Commands
{
    public class CommandContext
    {
        public object? Parameter { get; }
        public IServiceProvider Services { get; }
        public CancellationToken CancellationToken { get; }

        public CommandContext(IServiceProvider services, object? parameter = null, CancellationToken cancellationToken = default)
        {
            Services = services;
            Parameter = parameter;
            CancellationToken = cancellationToken;
        }

        public CommandContext(object? parameter = null, CancellationToken cancellationToken = default)
        {
            Parameter = parameter;
            CancellationToken = cancellationToken;
        }

        public T GetService<T>() where T : notnull
        {
            return (T)Services.GetRequiredService(typeof(T));
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
