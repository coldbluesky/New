using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Zhaoxi.AirCompression.Components
{
    public class ComponentBase : UserControl
    {
        public string ComponentId { get; set; } = "";
        public int ComponentIndex { get; set; } = -1;
        public int SelectState
        {
            get { return (int)GetValue(SelectStateProperty); }
            set { SetValue(SelectStateProperty, value); }
        }
        public static readonly DependencyProperty SelectStateProperty =
            DependencyProperty.Register("SelectState", typeof(int), typeof(ComponentBase), new PropertyMetadata(0, new PropertyChangedCallback(OnSelectStateChanged)));
        private static void OnSelectStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ComponentBase).IsSelected = false;
            if ((d as ComponentBase).ComponentIndex.ToString() == e.NewValue.ToString())
            {
                (d as ComponentBase).IsSelected = true;
            }
        }


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ComponentBase), new PropertyMetadata(false));


        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(ComponentBase), new PropertyMetadata(false));


        public bool ShowAlarm
        {
            get { return (bool)GetValue(ShowAlarmProperty); }
            set { SetValue(ShowAlarmProperty, value); }
        }
        public static readonly DependencyProperty ShowAlarmProperty =
            DependencyProperty.Register("ShowAlarm", typeof(bool), typeof(ComponentBase), new PropertyMetadata(false));

        //public string AlarmInfo
        //{
        //    get { return (string)GetValue(AlarmInfoProperty); }
        //    set { SetValue(AlarmInfoProperty, value); }
        //}
        //public static readonly DependencyProperty AlarmInfoProperty =
        //    DependencyProperty.Register("AlarmInfo", typeof(string), typeof(ComponentBase), new PropertyMetadata(""));



        public string SourceName
        {
            get { return (string)GetValue(SourceNameProperty); }
            set { SetValue(SourceNameProperty, value); }
        }
        public static readonly DependencyProperty SourceNameProperty =
            DependencyProperty.Register("SourceName", typeof(string), typeof(ComponentBase), new PropertyMetadata("Devices", new PropertyChangedCallback(OnPropertyChanged)));

        public string DeviceNum
        {
            get { return (string)GetValue(DeviceNumProperty); }
            set { SetValue(DeviceNumProperty, value); }
        }
        public static readonly DependencyProperty DeviceNumProperty =
            DependencyProperty.Register("DeviceNum", typeof(string), typeof(ComponentBase), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (d as ComponentBase);
            if (string.IsNullOrEmpty(obj.SourceName) ||
                string.IsNullOrEmpty(obj.DeviceNum)) return;

            var binding = new Binding();
            binding.Path = new PropertyPath($"DataContext.{obj.SourceName}[{obj.DeviceNum}]");
            binding.RelativeSource = new RelativeSource { AncestorType = typeof(Window), Mode = RelativeSourceMode.FindAncestor };

            BindingOperations.SetBinding(d, DataContextProperty, binding);


            binding = new Binding();
            binding.Path = new PropertyPath("State");
            binding.Converter = new IntToBoolConverter();

            BindingOperations.SetBinding(d, ShowAlarmProperty, binding);

            //binding = new Binding();
            //binding.Path = new PropertyPath("AlarmInfo");
            //binding.Converter = new IntToBoolConverter();

            //BindingOperations.SetBinding(d, ShowAlarmProperty, binding);

            Task.Run(async () =>
            {
                await Task.Delay(1);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (obj.IsSelected)
                        obj.Command?.Execute(new object[] { obj.DataContext, obj.ComponentIndex, obj.ComponentId });
                });
            });
        }



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ComponentBase), new PropertyMetadata(null));
    }

    class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == "3")
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
