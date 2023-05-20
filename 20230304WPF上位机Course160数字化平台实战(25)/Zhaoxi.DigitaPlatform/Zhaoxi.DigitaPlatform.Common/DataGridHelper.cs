using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Zhaoxi.DigitaPlatform.Common
{
    public class DataGridHelper
    {
        public static ObservableCollection<DataGridColumn> GetColumns(DependencyObject obj)
        {
            return (ObservableCollection<DataGridColumn>)obj.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject obj, ObservableCollection<DataGridColumn> value)
        {
            obj.SetValue(ColumnsProperty, value);
        }
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGridHelper), new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = (d as DataGrid);
            DataGridHelper.GetColumns(d).CollectionChanged += (se, ev) =>
            {
                dataGrid = (d as DataGrid);
                dataGrid.Columns.Clear();

                foreach (var item in DataGridHelper.GetColumns(dataGrid))
                {
                    dataGrid.Columns.Add(item);
                }
            };

            foreach (var item in DataGridHelper.GetColumns(d))
            {
                dataGrid.Columns.Add(item);
            }
        }
    }
}
