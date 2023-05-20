using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Wpf.Ui.Controls;

namespace WpfApp1
{
    /// <summary>
    /// DatagridAdd.xaml 的交互逻辑
    /// </summary>
    public partial class DatagridAdd : Window
    {
        ObservableCollection<User1> user = new ObservableCollection<User1>();
        public DatagridAdd()
        {
            InitializeComponent();
            User1 newUser = new User1
            {
                Id = user.Count + 1, // 设置新用户的 Id
                Name = "lin",
                Age = 0
            };
            user.Add(newUser);
            user.Add(newUser);

            userGrid.ItemsSource = user;
        }
        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // 从单元格中获取特定列的DataGridCell对象
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // 如果单元格当前未可视化，则强制其可视化
                    userGrid.ScrollIntoView(rowContainer, userGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        // 辅助函数，用于获取DataGridRow对象
        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)userGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // 如果行当前未可视化，则强制其可视化
                userGrid.UpdateLayout();
                userGrid.ScrollIntoView(userGrid.Items[index]);
                row = (DataGridRow)userGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        // 辅助函数，用于获取指定类型的可视化子元素
        public T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual visual = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = visual as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(visual);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            User1 newUser = new User1
            {
                Id = user.Count + 1, // 设置新用户的 Id
                Name = "",
                Age =222
            };
            user.Insert(0, newUser);
            userGrid.SelectedIndex = 0;

            DataGridCell cell = GetCell(0, 0);

            // 将单元格设置为编辑模式
            cell.Focus();
            cell.IsEditing = true;
            FrameworkElement element = cell.Content as FrameworkElement;
            if (element != null)
            {
                element.Focus();
            }
            //cell.LostFocus += ;
        }

        private string isPass(string name)
        {
            string msg = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                msg = "不能为空";
            }
            return msg;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string a = "张三，李四";
            string b = "张三";
            if (a.Contains(b))
            {
                box.Text = "通过！";
            }
            else
            {
                box.Text = "不通过！";

            }
        }
    }

    public class User1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
