using GalaSoft.MvvmLight.Messaging;
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
using Zhaoxi.AirCompression.View.DialogPages;
using Zhaoxi.AirCompression.ViewModel;

namespace Zhaoxi.AirCompression.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            // 配合模拟组态页面中的设备选择匹配功能
            Messenger.Default.Register<string>(this, "ChangeDevice", _ =>
            {
                ChooseDeviceWindow changeDeviceWindow = new ChooseDeviceWindow();
                changeDeviceWindow.Owner = this;

                if (changeDeviceWindow.ShowDialog() == true)
                    Messenger.Default.Send<string>("", "ChangeDeviceResult");
            });

            // 配合系统配置页面中的添加修改设备信息功能
            Messenger.Default.Register<string>(this, "ModifyDevice", _ =>
            {
                ModifyDeviceWindow modifyDeviceWindow = new ModifyDeviceWindow();
                modifyDeviceWindow.Owner = this;

                modifyDeviceWindow.ShowDialog();
            });
            // 配合系统配置页面中的添加修改点位信息功能
            Messenger.Default.Register<string>(this, "ModifyVariable", _ =>
            {
                ModifyVariableWindow modifyDeviceWindow = new ModifyVariableWindow();
                modifyDeviceWindow.Owner = this;

                modifyDeviceWindow.ShowDialog();
            });

            // 配合趋势曲线页面中的添加Y坐标轴功能
            Messenger.Default.Register<string>(this, "AddAxisY", _ =>
            {
                AddAxisYWindow addAxisYWindow = new AddAxisYWindow();
                addAxisYWindow.Owner = this;

                addAxisYWindow.ShowDialog();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModelLocator.Cleanup<MainViewModel>();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ?
                WindowState.Normal : WindowState.Maximized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // 如果关闭需要进行业务逻辑判断，最好在VM里用命令
            this.Close();
        }


    }
}
