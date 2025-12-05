
using CommandSystem.Console.Core;

namespace CommandSystem.Console.Commands
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq.Expressions;

    public static class ConsoleCommandFactoryHelper
    {
        private static readonly Dictionary<Type, Func<IServiceProvider, ConsoleCommandCatalog?, IConsoleCommand>> _factoryCache = new();

        public static IConsoleCommand CreateCommandInstance(
            IServiceProvider serviceProvider,
            Type commandType,
            ConsoleCommandCatalog? catalog = null)
        {
            if (!_factoryCache.TryGetValue(commandType, out var factory))
            {
                factory = BuildFactory(commandType);
                _factoryCache[commandType] = factory;
            }

            var instance = factory(serviceProvider, catalog);
            RegisterInCatalog(catalog, instance);
            return instance;
        }

        private static Func<IServiceProvider, ConsoleCommandCatalog?, IConsoleCommand> BuildFactory(Type commandType)
        {
            var serviceProviderParam = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var catalogParam = Expression.Parameter(typeof(ConsoleCommandCatalog), "catalog");

            var constructor = commandType.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (constructor == null)
                throw new InvalidOperationException($"Тип команды {commandType.Name} не имеет доступных конструкторов.");

            var constructorArgs = constructor.GetParameters().Select(param =>
            {
                if (param.ParameterType == typeof(ConsoleCommandCatalog))
                {
                    return (Expression)catalogParam;
                }

                // services.GetService(typeof(T))
                var getServiceMethod = typeof(IServiceProvider)
                    .GetMethod(nameof(IServiceProvider.GetService))!;

                var serviceCall = Expression.Call(serviceProviderParam, getServiceMethod, Expression.Constant(param.ParameterType));

                Expression serviceOrDefault;

                if (IsNullable(param.ParameterType) || param.HasDefaultValue)
                {
                    // Если можно null или есть дефолтное значение
                    var defaultValue = param.HasDefaultValue
                       ? (Expression)Expression.Constant(param.DefaultValue, param.ParameterType)
                       : Expression.Default(param.ParameterType);

                    serviceOrDefault = Expression.Coalesce(
                        Expression.Convert(serviceCall, param.ParameterType),
                        defaultValue
                    );
                }
                else
                {
                    // Если нельзя null — кидаем исключение, если сервис не найден
                    var notNullCheck = Expression.Block(
                        Expression.IfThen(
                            Expression.Equal(serviceCall, Expression.Constant(null)),
                            Expression.Throw(
                                Expression.New(typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) })!,
                                    Expression.Constant($"Не удалось разрешить обязательный параметр '{param.Name}' типа {param.ParameterType.Name} для команды {commandType.Name}."))
                            )
                        ),
                        Expression.Convert(serviceCall, param.ParameterType)
                    );

                    serviceOrDefault = notNullCheck;
                }

                return serviceOrDefault;
            }).ToArray();

            var newExpression = Expression.New(constructor, constructorArgs);
            var lambda = Expression.Lambda<Func<IServiceProvider, ConsoleCommandCatalog?, IConsoleCommand>>(newExpression, serviceProviderParam, catalogParam);
            return lambda.Compile();
        }

        private static bool IsNullable(Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        private static void RegisterInCatalog(ConsoleCommandCatalog? catalog, IConsoleCommand command)
        {
            if (catalog == null)
                return;

            catalog.AddCommand(command.Name, command.Description, command.GetType(), command.Aliases);
            //foreach (var alias in command.Aliases)
            //{
            //    catalog.AddCommand(alias, command.GetType());
            //}
        }
    }
}
