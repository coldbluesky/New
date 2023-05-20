using Microsoft.SqlServer.Server;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Interop.WinDef;

namespace WpfApp1.UserControls
{
    /// <summary>
    /// TimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class TimePicker : UserControl
    {
        private TextBox currentTextBox;
        public TimePicker()
        {
            InitializeComponent();
            UpBtn.Click += UpKeyDown;
            DownBtn.Click += DownKeyDown;
            HourBox.GotFocus += TextBoxGotFocus;
            MinuteBox.GotFocus += TextBoxGotFocus;
            SecondBox.GotFocus += TextBoxGotFocus;
            HourBox.LostFocus += TextBoxLostFocus;
            MinuteBox.LostFocus += TextBoxLostFocus;
            SecondBox.LostFocus += TextBoxLostFocus;
            this.GotFocus += TimePickerGotFocus;
            this.LostFocus += TimePickerLostFocus;
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Background = new SolidColorBrush(Colors.Transparent);
            textBox.Foreground = new SolidColorBrush(Colors.Black);
        }
        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            currentTextBox = textBox;
            textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078d7"));
            textBox.Foreground = new SolidColorBrush(Colors.White);
            FocusManager.SetFocusedElement(this, currentTextBox);


        }
        private void TimePickerLostFocus(object sender, RoutedEventArgs e)
        {
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 234, 234, 234));
            NumIncreaseVisible = Visibility.Collapsed;
            Time = Hour + ":" + Minute + ":" + Second;
        }

        private void TimePickerGotFocus(object sender, RoutedEventArgs e)
        {

            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 120, 215));
            NumIncreaseVisible = Visibility.Visible;
        }
     

        private void UpKeyDown(object sender, RoutedEventArgs e)
        {
            if (currentTextBox == null)
            {
                return;
            }
            int count = Convert.ToInt32(currentTextBox.Text);
            if (currentTextBox.Name == "HourBox")
            {
                if (count >= 23)
                {
                    count = 0;
                }
                else
                {
                    count += 1;
                }
                Hour = count.ToString("00");
            }
            else
            {
                if (count >= 59)
                {
                    count = 0;
                }
                else
                {
                    count += 1;
                }
                if (currentTextBox.Name == "MinuteBox")
                {
                    Minute = count.ToString("00");
                }
                else
                {
                    Second = count.ToString("00");
                }
            }
           
            currentTextBox.Text = count.ToString("00");
        }

        private void DownKeyDown(object sender, RoutedEventArgs e)
        {
            if (currentTextBox == null)
            {
                return;
            }
            int count = Convert.ToInt32(currentTextBox.Text);

            if (currentTextBox.Name == "HourBox")
            {
                if (count <= 0)
                {
                    count = 23;
                }
                else
                {
                    count -= 1;
                }
                Hour = count.ToString("00");
            }
            else
            {
                if (count <= 0)
                {
                    count = 59;
                }
                else
                {
                    count -= 1;
                }
                if (currentTextBox.Name == "MinuteBox")
                {
                    Minute = count.ToString("00");
                }
                else
                {
                    Second = count.ToString("00");
                }
            }
           
            currentTextBox.Text = count.ToString("00");
        }

     
        public String Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hour.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(TimePicker), new PropertyMetadata());
        public string Hour
        {
            get { return (string)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hour.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour", typeof(string), typeof(TimePicker), new PropertyMetadata(DateTime.Now.Hour.ToString("00")));


        public string Minute
        {
            get { return (string)GetValue(MinuteProperty); }
            set { SetValue(MinuteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minute.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register("Minute", typeof(string), typeof(TimePicker), new PropertyMetadata(DateTime.Now.Minute.ToString("00")));


        public string Second
        {
            get { return (string)GetValue(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Second.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondProperty =
            DependencyProperty.Register("Second", typeof(string), typeof(TimePicker), new PropertyMetadata(DateTime.Now.Second.ToString("00")));
        public Visibility NumIncreaseVisible
        {
            get { return (Visibility)GetValue(NumIncreaseVisibleProperty); }
            set { SetValue(NumIncreaseVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumIncreaseVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumIncreaseVisibleProperty =
            DependencyProperty.Register("NumIncreaseVisible", typeof(Visibility), typeof(TimePicker), new PropertyMetadata(Visibility.Collapsed));

       
    }
}
