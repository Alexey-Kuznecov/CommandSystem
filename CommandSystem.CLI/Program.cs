// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using CommandSystem.Console.Core;
using CommandSystem.Console.Commands;
using CommandSystem.Console.Integration;
using CommandSystem.CLI;
using CommandSystem.Commands;
using CommandSystem.Console.Integration.CommandSystem.Console.Integration;

namespace CommandSystem
{
    public class Program
    {
        private static readonly CancellationTokenSource _cts = new();

        private static async Task Main(string[] args)
        {          
            // Реакция на закрытие окна консоли или Environment.Exit
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                System.Console.WriteLine("ProcessExit -> Отмена запрошена.");
                _cts.Cancel(); // Сигнал всей системе
            };

            // 1. Настраиваем контейнер сервисов
            var services = new ServiceCollection();
            var lifetime = new ConsoleApplicationLifetime();
            services.AddSingleton(lifetime);

            // Регистрация сервисов
            services.AddSingleton<IConsoleOutput, ConsoleOutput>();
            services.AddSingleton<IConsoleCommandRegistry, ConsoleCommandRegistry>();
            services.AddSingleton<IConsoleCommandInvoker, ConsoleCommandInvoker>();
            services.AddSingleton<ConsoleCommandFactory>();
            services.AddSingleton<ConsoleCommandDispatcher>(); // <-- теперь у нас правильный Dispatcher

            // Копирование — вынесено в модуль
            services.AddCopying();

            var serviceProvider = services.BuildServiceProvider();

            // 2. Получаем нужные сервисы
            var dispatcher = serviceProvider.GetRequiredService<ConsoleCommandDispatcher>();
            var output = serviceProvider.GetRequiredService<IConsoleOutput>();
            var loadedCommands = ConsoleCommandDiscovery.DiscoverCommands(serviceProvider, new ConsoleCommandCatalog(), output);
            foreach (var command in loadedCommands)
            {
                dispatcher.RegisterCommand(command);
            }

            // 4. Запускаем консольный цикл
            output.WriteLine("Добро пожаловать в консольную тестовую систему команд!");
            output.WriteLine("Введите 'echo что-нибудь' для теста.\n");

            while (lifetime.IsRunning)
            {
                System.Console.Write("> ");
                var inputLine = System.Console.ReadLine();
                if (string.IsNullOrWhiteSpace(inputLine))
                    continue;

                var parts = ParseHelper.ParseArguments(inputLine);
                var commandName = parts[0];
                var commandArgs = parts.Skip(1).ToArray();
                var cts = new CancellationTokenSource();
                // Прерываем токен по Ctrl+C
                System.Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true; // Не завершаем процесс
                    cts.Cancel();    // Прерываем через токен
                };

                var context = new ConsoleCommandContext(serviceProvider, output, commandArgs);
                await dispatcher.ExecuteCommandAsync(commandName, context, cts.Token);
            }

            await dispatcher.FinalizeAllCommandsAsync();
            lifetime.NotifyStopped();
            output.Write("Консоль завершена.");
        }
    }
}