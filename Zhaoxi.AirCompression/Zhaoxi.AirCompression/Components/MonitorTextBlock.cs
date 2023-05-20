using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Zhaoxi.AirCompression.Components
{
    public class MonitorTextBlock : TextBlock
    {
        public string SourceName
        {
            get { return (string)GetValue(SourceNameProperty); }
            set { SetValue(SourceNameProperty, value); }
        }
        public static readonly DependencyProperty SourceNameProperty =
            DependencyProperty.Register("SourceName", typeof(string), typeof(MonitorTextBlock), new PropertyMetadata("Devices", new PropertyChangedCallback(OnPropertyChanged)));

        public string DeviceNum
        {
            get { return (string)GetValue(DeviceNumProperty); }
            set { SetValue(DeviceNumProperty, value); }
        }
        public static readonly DependencyProperty DeviceNumProperty =
            DependencyProperty.Register("DeviceNum", typeof(string), typeof(MonitorTextBlock), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));

        public string ValueNum
        {
            get { return (string)GetValue(ValueNumProperty); }
            set { SetValue(ValueNumProperty, value); }
        }
        public static readonly DependencyProperty ValueNumProperty =
            DependencyProperty.Register("ValueNum", typeof(string), typeof(MonitorTextBlock), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));


        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (d as MonitorTextBlock);
            if (string.IsNullOrEmpty(obj.SourceName) ||
                string.IsNullOrEmpty(obj.DeviceNum) ||
                string.IsNullOrEmpty(obj.ValueNum)) return;

            var binding = new Binding();
            binding.Path = new PropertyPath($"{obj.SourceName}[{obj.DeviceNum}][{obj.ValueNum}].CurrentValue");
            // 当没有任何值的时候   显示“--“
            binding.Converter = new NullStringConverter();

            BindingOperations.SetBinding(d, TextProperty, binding);
        }
    }

    class NullStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return "--";
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
