using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace lian
{
    /// <summary>
    /// SubWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SubWindow : Window
    {
        
        public SubWindow()
        {
            InitializeComponent();
        }
        public delegate void TurnOff();
        public static event TurnOff TurnOffEven;
        private void b_Click(object sender, RoutedEventArgs e)
        {

            Updata();
        }

        private void Updata()
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {

                var toggle = (ToggleButton)Application.Current.MainWindow.FindName("toggle");

                toggle.IsChecked = false;
            }));
            
        }
    }
}
