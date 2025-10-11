using ScottPlot.Plottables;
using Svetokop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Svetokop.Views
{
    public partial class ProgressView : UserControl
    {
        private DataStreamer? _streamer;           // не readonly — инициализируем в Loaded
        private ProgressViewModel? _vm;
        private readonly DispatcherTimer _renderTimer;

        public ProgressView()
        {
            InitializeComponent();

            Loaded += ProgressView_Loaded;
            Unloaded += ProgressView_Unloaded;

            // таймер рендера (DispatchTimer — на UI thread)
            _renderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _renderTimer.Tick += (_, __) => SpeedPlot.Refresh();
        }

        private void ProgressView_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ProgressViewModel;
            if (_vm == null) return;

            var plot = SpeedPlot.Plot;
            plot.Title("Скорость копирования");
            plot.YLabel("MB/s");
            plot.XLabel("Время (s)");

            // создаём DataStreamer (ScottPlot v5): показываем последние 300 точек
            _streamer = plot.Add.DataStreamer(points: 50000);

            // пусть стример сам управляет шкалой (по необходимости можно настроить вручную)
            _streamer.ManageAxisLimits = true;
            _streamer.ViewWipeRight(); // опция представления (по желанию)

            // опционально: начальные лимиты Y (если хочешь фиксированный старт)
            plot.Axes.SetLimitsY(0, 100);

            // подписка на события из VM
            _vm.SpeedSampleAvailable += OnSpeedSample;

            _renderTimer.Start();
        }

        private void ProgressView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_vm != null) _vm.SpeedSampleAvailable -= OnSpeedSample;
            _renderTimer.Stop();
        }

        private double _xIndex = 0;

        private void OnSpeedSample(double mbps)
        {
            Dispatcher.Invoke(() =>
            {
                if (_streamer == null) return;
                _streamer.Add(mbps);

                //_xIndex++;
                //var plot = SpeedPlot.Plot;
                //plot.Axes.SetLimitsX(Math.Max(0, _xIndex - 300), _xIndex); // показывает последние 300 точек
            });
        }
    }
}
