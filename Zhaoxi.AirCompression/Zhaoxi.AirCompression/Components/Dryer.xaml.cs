using System;
using System.Collections.Generic;
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

namespace Zhaoxi.AirCompression.Components
{
    /// <summary>
    /// Dryer.xaml 的交互逻辑
    /// </summary>
    public partial class Dryer : UserControl
    {
        public bool ShowAlarm
        {
            get { return (bool)GetValue(ShowAlarmProperty); }
            set { SetValue(ShowAlarmProperty, value); }
        }
        public static readonly DependencyProperty ShowAlarmProperty =
            DependencyProperty.Register("ShowAlarm", typeof(bool), typeof(Dryer), new PropertyMetadata(false));


        public string SourceName
        {
            get { return (string)GetValue(SourceNameProperty); }
            set { SetValue(SourceNameProperty, value); }
        }
        public static readonly DependencyProperty SourceNameProperty =
            DependencyProperty.Register("SourceName", typeof(string), typeof(Dryer), new PropertyMetadata("Devices", new PropertyChangedCallback(OnPropertyChanged)));

        public string DeviceNum
        {
            get { return (string)GetValue(DeviceNumProperty); }
            set { SetValue(DeviceNumProperty, value); }
        }
        public static readonly DependencyProperty DeviceNumProperty =
            DependencyProperty.Register("DeviceNum", typeof(string), typeof(Dryer), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (d as Dryer);
            if (string.IsNullOrEmpty(obj.SourceName) ||
                string.IsNullOrEmpty(obj.DeviceNum)) return;

            var binding = new Binding();
            binding.Path = new PropertyPath($"{obj.SourceName}[{obj.DeviceNum}]");

            BindingOperations.SetBinding(d, DataContextProperty, binding);
        }
        public Dryer()
        {
            InitializeComponent();
        }
    }
}
