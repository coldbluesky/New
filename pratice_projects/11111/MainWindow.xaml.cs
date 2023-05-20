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

namespace _11111
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<string> l = new List<string>();
            l.Add("w");
            l.Add("sb");
            l.Add("w");
            l.Add("w");
            //box.ItemsSource = l;
            //foreach (string i in l)
            //{
            //    box.Items.Add(i);
            //}
           
        }

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string date = myDT.Text.ToString();
            //b.Text = date;
            string date = myDT.Text.ToString();
            if (date != null)
            {
                b.Text = date;
            }
            else
            {
                b.Text = "空啊";
            }

        }
    }
}
