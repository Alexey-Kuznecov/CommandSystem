using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Svetokop.Behaviors
{
    public static class ListViewSelectedItemsBehavior
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(ListViewSelectedItemsBehavior),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        public static void SetSelectedItems(DependencyObject element, IList value) =>
            element.SetValue(SelectedItemsProperty, value);

        public static IList GetSelectedItems(DependencyObject element) =>
            (IList)element.GetValue(SelectedItemsProperty);

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                listView.SelectionChanged -= ListView_SelectionChanged;
                listView.SelectionChanged += ListView_SelectionChanged;
            }
        }

        private static void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                SetSelectedItems(listView, listView.SelectedItems);
            }
        }
    }
}
