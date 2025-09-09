using AlexeyKuznetsov.Logger;
using CommandSystem.CopyTester.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnityCommander.Copying;
using UnityCommander.Copying.Core;
using UnityCommander.Copying.Handler;
using UnityCommander.Copying.Progress;
using UnityCommander.Copying.Reporting;
using UnityCommander.Copying.Strategies;

namespace CommandSystem.CopyTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
            var mainVM = serviceProvider.GetRequiredService<MainViewModel>();
            this.DataContext = mainVM;
            InitializeComponent();
        }
    }
}