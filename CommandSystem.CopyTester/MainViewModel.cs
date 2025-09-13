
using AlexeyKuznetsov.Logger;
using CommandSystem.CopyTester.ViewModels;
using CommandSystem.Gui.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Collections.ObjectModel;

using UnityCommander.Copying;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Sessions;
using UnityCommander.Copying.Settings;
using UnityCommander.Copying.Strategies;

namespace CommandSystem.CopyTester
{
    public class MainViewModel : ObservableObject
    {
        private readonly CopyManager _copyManager;
        private readonly IProgressReporter _reporter;
        private ObservableObject _currentViewModel;
        public CopySetupViewModel SetupVM { get; }
        public CopyReportViewModel ReportVM { get; }
        public CopyProgressViewModel ProgressVM { get; private set; }
        public ObservableCollection<CopySessionService> ActiveSessions { get; } = new();
        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public MainViewModel()
        {
            var services = new ServiceCollection();

            // Сессия копирования (одиночный сервис, чтобы все VM делились состоянием)
            services.AddSingleton<CopySessionService>();

            // Менеджер копирования
            services.AddSingleton<CopyManager>();

            // Стратегии и трекеры
            services.AddSingleton<IProgressCalculator, ProgressCalculator>();
            services.AddSingleton<ISpeedCalculator, SpeedCalculator>();
            services.AddSingleton<IFileCopier, StreamFileCopier>();
            services.AddSingleton<IProgressTracker, ProgressTracker>();
            services.AddSingleton<IProgressReporter, GuiProgressReporter>();
            services.AddSingleton<ICopyErrorHandler, LoggerCopyErrorHandler>();
            services.AddSingleton<ICopySuccessHandler, GuiCopySuccessHandler>();
            services.AddSingleton<IFileCopyPlanner, DefaultFileCopyPlanner>();
            services.AddSingleton<ILogger, FileLogger>();
            // VM
            services.AddTransient<MainViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            _copyManager = serviceProvider.GetRequiredService<CopyManager>();
            _reporter = serviceProvider.GetRequiredService<IProgressReporter>();

            SetupVM = new CopySetupViewModel(this);
            CurrentViewModel = SetupVM;
        }

        // Добавление сессии в очередь и запуск
        public void StartQuickCopy(string source, string destination)
        {
            //var session = new CopySessionService();
            //RunCopySession(session);
        }

        public void StartWizardCopy(CopySessionService session)
        {
            RunCopySession(session);
        }

        private void RunCopySession(CopySessionService session)
        {
            this.ActiveSessions.Add(session);
            this.ProgressVM = new CopyProgressViewModel(session, _reporter, this);
            this.CurrentViewModel = ProgressVM;
            var cts = new CancellationTokenSource();
            //session.CancellationToken = cts.Token;
            //_ = _copyManager.CopyFilesAsyncOld(session, SetupVM.ToCopyOptions(), cts.Token);
        }
    }
}
