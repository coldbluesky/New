using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace Zhaoxi.DigitaPlatform.Common.Converter
{
    public class FocuseToSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            (value as TextBoxBase).GotFocus -= FocuseToSelectedConverter_GotFocus;
            (value as TextBoxBase).GotFocus += FocuseToSelectedConverter_GotFocus;


            return Visibility.Visible;
        }

        private void FocuseToSelectedConverter_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ((sender as TextBoxBase).Tag as ListBoxItem).IsSelected = true;
            //.SetValue(ListViewItem.IsSelectedProperty, true);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
