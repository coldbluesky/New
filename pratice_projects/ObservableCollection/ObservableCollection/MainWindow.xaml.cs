using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ObservableCollection
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            person1.Add(new Person() { Name = "张三" });
            person1.Add(new Person() { Name = "李四" });
            listbind.ItemsSource = person1;
            person2.Add(new Person() { Name = "张三" });
            person2.Add(new Person() { Name = "李四" });
            observbind.ItemsSource = person2;
        }

        private List<Person> person1 = new List<Person>();
        private ObservableCollection<Person> person2 = new ObservableCollection<Person>();

     

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            person1.Add(new Person() { Name = "王五" });
            person2.Add(new Person() { Name = "王五" });
        }
    }

    public class Person
    {
        public string Name { get; set; }
    }
}
