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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// APath.xaml 的交互逻辑
    /// </summary>
    public partial class APath : Window
    {
        public APath()
        {
            InitializeComponent();
            box.Text =Convert.ToString((int)Enum.Parse(typeof(Name), "Petter"));
        }
    }

    
       public enum Name 
        
        {
            Petter = 2,
        }
    
}
