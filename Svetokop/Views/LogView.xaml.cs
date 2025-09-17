using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Svetokop.Views
{
    /// <summary>
    /// Логика взаимодействия для LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();

            // Автопрокрутка вниз при изменении элементов
            LogListView.Loaded += (s, e) =>
            {
                if (LogListView.ItemsSource is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged += (sender, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add)
                            ScrollViewer.ScrollToEnd();
                    };
                }
            };
        }
    }
}
