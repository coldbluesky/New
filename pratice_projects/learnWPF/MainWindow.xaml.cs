using System;
using System.Collections.Generic;
using System.Collections;
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
using HslCommunication.Profinet.Omron;
using HslCommunication;
namespace learnWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ip = IPAdress.Text;
            int por = int.Parse(port.Text);
            string adr = adress.Text;
            OmronFinsNet omronFinsNet = new OmronFinsNet(ip,por);
            OperateResult<short> res = omronFinsNet.ReadInt16(adr);
            if (res.IsSuccess)
            {

                result.Text = DateTime.Now.ToString() + ":" + res.Content+"\n";
            }
            else
            {
                MessageBox.Show("连接失败");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string ip = IPAdress.Text;
            int por = int.Parse(port.Text);
            string adr = adress.Text;
            short d = short.Parse(data.Text);
            OmronFinsNet omronFinsNet = new OmronFinsNet(ip, por);
            OperateResult res = omronFinsNet.Write(adr,(short)d);
            if (res.IsSuccess)
            {
                MessageBox.Show("写入成功");

            }
            else
            {
                MessageBox.Show("连接失败");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> arrayList = new List<string>();
            arrayList.Add("a");
            arrayList.Add("b");
            arrayList.Add("c");
            arrayList.Add("d");
            MyC.ItemsSource = arrayList;
            MyC.SelectedIndex = 0;
            foreach (string i in arrayList)
            {
                Console.WriteLine(i);
            }
        }
    }
}
