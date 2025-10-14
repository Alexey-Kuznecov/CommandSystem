using AlexeyKuznetsov.Logger;
using CommandSystem.CopyTester.ViewModels;
using CommandSystem.Gui.MVVM;
using Svetokop.Services;
using UnityCommander.Copying;
using UnityCommander.Copying.Category;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using UnityCommander.Copying.Strategies;

namespace Svetokop.ViewModels
{
    public class CopyWindowViewModel : ObservableObject
    {
        private readonly CopyManager _copyManager;
        private CopySessionManager _copySessionManager;
        // Под-VM для отдельных областей
        public ProgressViewModel ProgressVM { get; }
        public FileListViewModel FileListVM { get; }
        public SettingsViewModel SettingsVM { get; }
        public LogViewModel LogVM { get; }
        public HistoryViewModel HistoryVM { get; }
        public MetricViewModel MetricVM { get; }

        public SpeedGraphViewModel SpeedGraphVM { get; }
        public CopyOptions copyOption => new CopyOptions
        {
            UseCategories = true,
            UseMultiThreading = true,
            MaxConсurrentTasks = 5,
            UseMetrics = true,
            UseDualChannels = true,
            // Новое
            BufferSize = 64 * 1024,
            MinBufferSize = 8 * 1024,
            UseProgressiveDiscovery = false,
            VerboseLogging = true
        };

        public CopyWindowViewModel()
        {
            // Логгер
            ILogger logger = new FileLogger();
            var categorizer = new NeuralCategorizer();
            // Стратегии и трекеры
            IProgressCalculator progressCalculator = new ProgressCalculator();
            ISpeedCalculator speedCalculator = new SpeedCalculator();
            IProgressTracker progressTracker = new ProgressTracker(progressCalculator, speedCalculator);
            IProgressReporter reporter = new GuiProgressReporter();
            IFileCopier fileCopier = new StreamFileCopier();
            ICopyErrorHandler errorHandler = new LoggerCopyErrorHandler(logger);
            ICopySuccessHandler successHandler = new GuiCopySuccessHandler();
            IFileCopyPlanner fileCopyPlanner = new DefaultFileCopyPlanner(categorizer);
            //ICopyReporter fileReporter = new CopyFileReporter(action => Application.Current.Dispatcher.Invoke(action));
            ICopyReporter fileReporter = new CopyFileReporter2();
            ICopyReporter logReporter = new CopyLogReporter();
            ICopyMetricsCollector metrics = new NullCopyMetricsCollector();
            _copySessionManager = new CopySessionManager(fileReporter, logReporter);
            
            // Менеджер копирования (собираем руками)
            _copyManager = new CopyManager(
                fileCopier,
                fileCopyPlanner,
                progressTracker,
                reporter,
                categorizer,
                consoleOutput: null,
                errorHandler: errorHandler,
                successHandler: successHandler,
                metrics
            );

            // Инициализация под-VM
            FileListVM = new FileListViewModel(fileReporter);
            ProgressVM = new ProgressViewModel(_copyManager, _copySessionManager);
            SettingsVM = new SettingsViewModel();
            HistoryVM = new HistoryViewModel(_copyManager);
            MetricVM = new MetricViewModel();
            LogVM = new LogViewModel(logReporter);
            ProgressVM.StartRequested += OnStartRequested;
            SpeedGraphVM = new SpeedGraphViewModel(_copyManager.ProgressStream);
        }

        private async Task OnStartRequested()
        {
            var source = SettingsVM.SourcePath;
            var destination = SettingsVM.DestinationPath;
            var session = _copySessionManager.CreateSession(source, destination);
            if (session != null)
                await _copyManager.CopyFilesAsync(source, destination, session, BuildSettings(SettingsVM, session));
        }

        public CompositeCopySettings BuildSettings(SettingsViewModel userSettings, CopySessionService session)
        {
            var composite = new CompositeCopySettings();
            // дефолтные
            composite.Add(SettingPriority.Default, opts =>
            {
                opts.UseMultiThreading = true;
                opts.MaxConсurrentTasks = 5;
                opts.UseCategories = true;
                opts.UseMetrics = true;
                opts.UseDualChannels = true;
                // Новое
                opts.BufferSize = 64 * 1024;
                opts.MinBufferSize = 8 * 1024;
                opts.UseProgressiveDiscovery = false;
                opts.VerboseLogging = true;
            });

            // сессионные
            composite.Add(SettingPriority.Session, opts =>
            {
                //session.ProgressStep = 10;
                //session.VerboseLogging = false;
                //opts.VerboseLogging = session.VerboseLogging;
            });

            // пользовательские (имеют больший приоритет)
            composite.Add(SettingPriority.User, opts =>
            {
                opts.MaxConсurrentTasks = userSettings.MaxConcurrentTasks; // переопределение
                opts.UseMultiThreading = userSettings.UseMultiThreading;
            });

            return composite;
        }
    }
}
