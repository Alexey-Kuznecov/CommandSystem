
using AlexeyKuznetsov.Logger;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Copying;
using UnityCommander.Copying.Core;
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
            //services.AddSingleton<IProgressCalculator>(new SmoothProgressCalculator(0.1));
            services.AddSingleton<IProgressCalculator, ProgressCalculator>();
            services.AddSingleton<ISpeedCalculator, SpeedCalculator>();
            services.AddSingleton<IProgressTracker, ProgressTracker>();
            services.AddSingleton<IFileCopier, StreamFileCopier>();
            services.AddSingleton<ILogger, FileLogger>();
            services.AddSingleton<IProgressReporter, ConsoleProgressReporter>();
            services.AddSingleton<ICopyErrorHandler, LoggerCopyErrorHandler>();
            services.AddSingleton<ICopySuccessHandler, GuiCopySuccessHandler>();
            services.AddSingleton<IFileCopyPlanner, DefaultFileCopyPlanner>();
            services.AddTransient<ICopyMetricsCollector, CopyMetricsCollector>();
            services.AddSingleton<CopyOptions>();
            services.AddSingleton<CopyManager>();

            return services;
        }
    }
}
