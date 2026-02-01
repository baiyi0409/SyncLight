 using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SyncLight.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return System.Windows.Media.Brushes.Transparent;

            // 处理 System.Windows.Media.Color
            if (value is System.Windows.Media.Color color)
            {
                return new SolidColorBrush(color);
            }

            // 处理 System.Drawing.Color
            if (value is System.Drawing.Color drawingColor)
            {
                return new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                    drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B));
            }

            // 处理字符串格式的颜色
            if (value is string colorString)
            {
                try
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    return (SolidColorBrush)converter.ConvertFromString(colorString);
                }
                catch
                {
                    return System.Windows.Media.Brushes.Transparent;
                }
            }

            return System.Windows.Media.Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
