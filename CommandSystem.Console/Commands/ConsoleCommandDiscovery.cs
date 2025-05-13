using System.Reflection;
using CommandSystem.Console.Commands;
using CommandSystem.Console.Core;
using CommandSystem.Console.Integration;
using Microsoft.Extensions.DependencyInjection; // <- Обязательно

public static class ConsoleCommandDiscovery
{
    private static readonly string _cmdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cmd");

    public static IEnumerable<IConsoleCommand> DiscoverCommands(IServiceProvider serviceProvider, ConsoleCommandCatalog catalog, IConsoleOutput? output = null)
    {
        if (!Directory.Exists(_cmdPath))
        {
            output?.WriteWarning($"Папка с командами не найдена: {_cmdPath}");
            yield break;
        }

        var assemblies = new List<Assembly>();
        var commands = new List<IConsoleCommand>();

        foreach (var dllPath in Directory.GetFiles(_cmdPath, "*.dll", SearchOption.TopDirectoryOnly))
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                assemblies.Add(assembly);
                output?.WriteLine($"Загружена сборка: {Path.GetFileName(dllPath)}");
            }
            catch (Exception ex)
            {
                output?.WriteError($"Ошибка при загрузке {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        var commandTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IConsoleCommand).IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<ConsoleCommandAttribute>() != null);

        foreach (var type in commandTypes)
        {
            try
            {
                var command = ConsoleCommandFactoryHelper.CreateCommandInstance(serviceProvider, type, catalog);
                commands.Add(command);
                output?.WriteLine($"Команда найдена и загружена: {type.Name}");
            }
            catch (Exception ex)
            {
                output?.WriteError($"Ошибка при создании команды {type.Name}: {ex.Message}");
            }
        }

        output?.WriteSuccess($"Всего загружено команд: {commands.Count}");

        foreach (var command in commands)
        {
            yield return command;
        }
    }
}
