
using AlexeyKuznetsov.Logger;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Copying;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Filtering;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Settings;
using UnityCommander.Copying.Strategies;

namespace CommandSystem.CLI
{
    public static class CopyingServicesConfigurator
    {
        public static IServiceCollection AddCopying(this IServiceCollection services)
        {
            services.AddTransient<IProgressTracker, ProgressTracker>();
            services.AddSingleton<IFileCopier, StreamFileCopier>();
            services.AddSingleton<ILogger, FileLogger>();
            services.AddTransient<IProgressReporter, ConsoleProgressReporter>();
            services.AddSingleton<ICopyErrorHandler, LoggerCopyErrorHandler>();
            services.AddSingleton<ICopySuccessHandler, GuiCopySuccessHandler>();
            services.AddSingleton<IFileCopyPlanner, DefaultFileCopyPlanner>();

            //var settings = new CompositeCopySetting(new List<ICopySetting>
            //{
            //    new RecursiveCopySetting(),  // Пример настройки рекурсивного копирования
            //    //new CopyAllToOneFolderSetting()  // Пример настройки копирования в одну папку
            //});

            //// Применяем настройки
            //settings.Apply(ref options);
            services.AddSingleton<CopyOptions>();
            services.AddTransient<CopyManager>();

            return services;
        }
    }
}
