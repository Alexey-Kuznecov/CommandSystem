using Svetokop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Svetacopy.ViewModels
{
    public static class VisualTreeHelpers
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        public static Rect GetPlotArea(this WpfGraphControl control)
        {
            var plotCanvas = control.Template.FindName("PART_PlotArea", control) as FrameworkElement;
            if (plotCanvas == null)
                return Rect.Empty;

            var transform = plotCanvas.TransformToAncestor(control);
            var rect = new Rect(transform.Transform(new Point(0, 0)), new Size(plotCanvas.ActualWidth, plotCanvas.ActualHeight));
            return rect;
        }
    }
}
