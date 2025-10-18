using AlexeyKuznetsov.Logger;
using CommandSystem.CopyTester.ViewModels;
using CommandSystem.Gui.MVVM;
using Svetokop.Services;
using System.CodeDom.Compiler;
using System.Numerics;
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
        private readonly OpenManager _openManager;
        private CopySessionManager _copySessionManager;
        // Под-VM для отдельных областей
        public ProgressViewModel ProgressVM { get; }
        public FileListViewModel FileListVM { get; }
        public SettingsViewModel SettingsVM { get; }
        public LogViewModel LogVM { get; }
        public HistoryViewModel HistoryVM { get; }
        public MetricViewModel MetricVM { get; }

        public SpeedGraphViewModel SpeedGraphVM { get; }

        public CopyWindowViewModel()
        {
            // Логгер
            ILogger logger = new FileLogger();
            var categorizer = new NeuralCategorizer();
            // Стратегии и трекеры
            IProgressCalculator progressCalculator = new ProgressCalculator();
            ISpeedCalculator speedCalculator = new SpeedCalculator();
            IProgressTracker progressTracker = new ProgressTracker(progressCalculator, speedCalculator);
            IProgressReporter guiReporter = new GuiProgressReporter();
            IProgressReporter aggregatedReporter = new AggregatedProgressReporter(guiReporter, TimeSpan.FromMilliseconds(1));
            IFileCopyPlanner fileCopyPlanner = new DefaultFileCopyPlanner(categorizer);
            ICopyExecutionStrategy copierExFactory = new ParallelExecutionStrategy();
            IFileCopierFactory copierFactory = new DefaultFileCopierFactory();
            ICopyReporter fileReporter = new Services.CopyFileReporter();
            ICopyExecutor excutor = new CopyExecutor();
            ICopyReporter logReporter = new CopyLogReporter();
            ICopyMetricsCollector metrics = new NullCopyMetricsCollector();
            _copySessionManager = new CopySessionManager(fileReporter, logReporter);

            _openManager = new OpenManager(
                fileCopyPlanner,
                excutor,
                copierFactory,
                progressTracker,
                aggregatedReporter,
                categorizer,
                metrics,
                logger
            );

            // Инициализация под-VM
            FileListVM = new FileListViewModel(fileReporter);
            ProgressVM = new ProgressViewModel(_openManager, _copySessionManager);
            SettingsVM = new SettingsViewModel();
            HistoryVM = new HistoryViewModel(_copyManager);
            MetricVM = new MetricViewModel();
            LogVM = new LogViewModel(logReporter);
            ProgressVM.StartRequested += OnStartRequested;
            SpeedGraphVM = new SpeedGraphViewModel(_openManager.ProgressStream);
        }

        private async Task OnStartRequested()
        {
            var source = SettingsVM.SourcePath;
            var destination = SettingsVM.DestinationPath;
            var session = _copySessionManager.CreateSession(source, destination);
            if (session != null)
                await _openManager.StartCopyAsync(source, destination, session, BuildSettings(SettingsVM, session));
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
                opts.UseDualChannels = false;
                opts.UseParallel = true;
                // Новое
                opts.BufferSize = 64 * 1024;
                opts.MinBufferSize = 8 * 1024;
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
                opts.UseParallel = userSettings.UseMultiThreading;
                opts.UseProgressiveDiscovery = false;
                opts.UseWinApi = false;
            });

            return composite;
        }
    }
}
