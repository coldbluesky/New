using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Student> students = new ObservableCollection<Student>();
        public MainWindow()
        {
            InitializeComponent();
            students.Add(new Student { Name="林德毅"});
            showBox.ItemsSource = students;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            students.Add(new Student { Name = "张三" });
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            
            if (showBox.SelectedItem !=null)
            {
                students.Remove(showBox.SelectedItem as Student);
            }
        }

        private void modify_Click(object sender, RoutedEventArgs e)
        {
            if (showBox.SelectedItem != null)
            {
                (showBox.SelectedItem as Student).Name = "改了";
            }
        }
    }

    public class Student:INotifyPropertyChanged
    {

      
        private string name;

        public string Name
        {
            get { return name; }
            set { 
               
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChange("Name");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChange(string proName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(proName));
            }
        }
    }

}
   
   

