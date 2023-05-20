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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Zhaoxi.DigitaPlatform.Common;
using Zhaoxi.DigitaPlatform.Views.Dialog;

namespace Zhaoxi.DigitaPlatform.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            if (new LoginView().ShowDialog() != true)
                Application.Current.Shutdown();

            InitializeComponent();

            // 注册，需要状态返回
            ActionManager.Register<object>("AAA",
                new Func<object, bool>(ShowConfigView));

            ActionManager.Register<object>("ShowRight",
                new Func<object, bool>(ShowRightView));

            ActionManager.Register<object>("ShowTrendAxis",
                new Func<object, bool>(ShowTrendAxisDialog));

            ActionManager.Register<object>("ShowTrendVars", 
                new Func<object, bool>(ShowTrendDeviceVars));

        }
        private bool ShowConfigView(object obj)
        {
            return ShowDialog(new ComponentConfigView() { Owner = this });
        }
        private bool ShowRightView(object obj)
        {
            return ShowDialog(new RightRemindDialog() { Owner = this });
        }
        private bool ShowTrendAxisDialog(object obj)
        {
            return ShowDialog(new TrendAxisEditDialog() { Owner = this, DataContext = obj });
        }
        private bool ShowTrendDeviceVars(object obj)
        {
            return ShowDialog(new TrendDeviceChooseDialog() { Owner = this, DataContext = obj });
        }

        private bool ShowDialog(Window dialog)
        {
            this.Effect = new BlurEffect() { Radius = 5 };
            bool state = dialog.ShowDialog() == true;
            this.Effect = null;
            return state;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
