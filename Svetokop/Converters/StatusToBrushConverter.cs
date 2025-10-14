
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using UnityCommander.Copying.Sessions;

namespace Svetokop.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileCopyStatus status)
            {
                return status switch
                {
                    FileCopyStatus.InProgress => Brushes.SteelBlue,
                    FileCopyStatus.Completed => Brushes.SeaGreen,
                    FileCopyStatus.Failed => Brushes.IndianRed,
                    _ => Brushes.Gray
                };
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
