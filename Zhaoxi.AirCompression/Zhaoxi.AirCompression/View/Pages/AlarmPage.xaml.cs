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
using Zhaoxi.AirCompression.ViewModel;

namespace Zhaoxi.AirCompression.View.Pages
{
    /// <summary>
    /// AlarmPage.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmPage : UserControl
    {
        public AlarmPage()
        {
            InitializeComponent();
            this.Unloaded += AlarmPage_Unloaded;
        }

        private void AlarmPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.Cleanup<AlarmViewModel>();
        }
    }
}
