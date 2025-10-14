
using CommandSystem.Gui.MVVM;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Media;
using UnityCommander.Copying.Core;
using UnityGraph;
using UnityGraph.DataPoints;
using UnityGraph.WPF;

namespace Svetokop.ViewModels
{
    public class SpeedGraphViewModel : ObservableObject
    {
        public bool _displayToolTip;
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; }

        public bool DisplayToolTip
        {
            get => _displayToolTip;
            set
            {
                SetProperty(ref _displayToolTip, value);
                OnPropertyChanged("DisplayToolTip");
            }
        }

        private readonly Stopwatch _watch = new();
                private int _initialSkipCount = 10;
        public SpeedGraphViewModel(IObservable<ProgressInfo> progressStream)
        {
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MaximumY = 60; // 60 MB/s
            Controller.Range.MinimumY = 1; // 1 MB/s
            Controller.Range.MaximumX = TimeSpan.FromSeconds(100);
            Controller.Range.AutoY = true;
            Controller.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.None;
            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries
            {
                Name = "Speed (MB/s)",
                StrokeThickness = 2,
                Stroke = Colors.DodgerBlue,
                Fill = new SolidColorBrush(Color.FromArgb(100, 30, 144, 255))
            });
            
            _watch.Start();

            progressStream
                .Sample(TimeSpan.FromMilliseconds(100))
                .ObserveOn(SynchronizationContext.Current!)
                .Subscribe(UpdateGraph);
        }

        private void UpdateGraph(ProgressInfo info)
        {
            if (_initialSkipCount > 0)
            {
                _initialSkipCount--;
                return;
            }
            DisplayToolTip = true;
            var x = _watch.Elapsed;
            var y = info.SpeedBytesPerSecond / 1024.0 / 1024.0;
            Controller.PushData(x, y);
        }
    }
}
