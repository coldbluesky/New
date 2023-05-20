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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// 平移.xaml 的交互逻辑
    /// </summary>
    public partial class 平移 : Window
    {
        public 平移()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = -50;
            animation.To = 110;
            animation.Duration = TimeSpan.FromSeconds(3);
            buttonTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
    }
}
