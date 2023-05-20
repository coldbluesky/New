using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Zhaoxi.DigitaPlatform.Entities;
using Zhaoxi.DigitaPlatform.IDataAccess;

using Zhaoxi.DigitaPlatform.Models;

namespace Zhaoxi.DigitaPlatform.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        public ObservableCollection<DataGridColumn> Columns { get; set; } =
            new ObservableCollection<DataGridColumn>();

        public ObservableCollection<RecordReadEntity> AllDatas { get; set; } =
            new ObservableCollection<RecordReadEntity>();

        // 所有可能显示的列，可供选择的列
        public ObservableCollection<DataGridColumnModel> AllColumms { get; set; } =
            new ObservableCollection<DataGridColumnModel>();

        public RelayCommand<DataGridColumnModel> ChooseColumnCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }

        ILocalDataAccess _localDataAccess;
        public ReportViewModel(ILocalDataAccess localDataAccess)
        {
            _localDataAccess = localDataAccess;

            // 初始化列列表
            AllColumms.Add(new DataGridColumnModel { Header = "设备编号", BindingPath = "DeviceNum", ColumnWidth = 70 });
            AllColumms.Add(new DataGridColumnModel { IsSelected = true, Header = "设备名称", BindingPath = "DeviceName", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "变量编号", BindingPath = "VariableNum", ColumnWidth = 70 });
            AllColumms.Add(new DataGridColumnModel { IsSelected = true, Header = "变量名称", BindingPath = "VariableName", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "最新值", BindingPath = "LastValue", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "平均值", BindingPath = "AvgValue", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "最大值", BindingPath = "MaxValue", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "最小值", BindingPath = "MinValue", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "报警触发次数", BindingPath = "AlarmCount", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "联控触发次数", BindingPath = "UnionCount", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "记录次数", BindingPath = "RecordCount", ColumnWidth = 80 });
            AllColumms.Add(new DataGridColumnModel { Header = "最后记录时间", BindingPath = "LastTime", ColumnWidth = 120 });

            // 添加默认列
            foreach (var item in AllColumms)
                ChangedColumn(item);

            // 获取所有数据
            Refresh();

            ChooseColumnCommand = new RelayCommand<DataGridColumnModel>(ChangedColumn);
            ExportCommand = new RelayCommand(Exeport);
        }
        private int index = 0;
        private void ChangedColumn(DataGridColumnModel model)
        {
            if (model.IsSelected)
            {
                model.Index = index++;
                Style style = new System.Windows.Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
                style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
                Columns.Add(new DataGridTextColumn
                {
                    Header = model.Header,
                    Binding = new Binding(model.BindingPath),
                    MinWidth = model.ColumnWidth,
                    ElementStyle = style
                });
            }
            else
            {
                var column = Columns.FirstOrDefault(c => c.Header.ToString() == model.Header);
                Columns.Remove(column);
            }
        }

        private void Exeport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Report-" + DateTime.Now.ToString("yyyyMMddHHmmssFFF") + ".csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                // 灵活配置的列，列没有顺序
                List<DataGridColumnModel> cs = AllColumms.Where(c => c.IsSelected).OrderBy(c => c.Index).ToList();

                // 列头
                string datas = string.Join(",", cs.Select(c => c.Header)) + "\r\n";

                // 数据
                // 遍历所有数据行
                foreach (var item in AllDatas)
                {
                    // 遍历所有数据列
                    foreach (var col in cs)
                    {
                        // 确定当前列绑定的属性名称
                        // 需求：根据字符串名称获取对应名称的属性值
                        PropertyInfo pi = item.GetType().GetProperty(col.BindingPath, BindingFlags.Instance | BindingFlags.Public);

                        var v = pi.GetValue(item);
                        datas += v == null ? "," : v.ToString() + ",";
                    }
                    datas.Remove(datas.Length - 1);
                    datas += "\r\n";
                }

                System.IO.File.WriteAllText(saveFileDialog.FileName, datas, Encoding.UTF8);
            }
        }

        public ICommand RefreshCommand
        {
            get => new RelayCommand(() =>
            {
                Refresh();
            });
        }

        private void Refresh()
        {
            // 从数据库来的
            var datas = _localDataAccess.GetRecords();

            // 第一种方式
            //AllDatas.Clear();
            //datas.ForEach(d => AllDatas.Add(d));

            // 第二种方式
            AllDatas = new ObservableCollection<RecordReadEntity>(datas);
            this.RaisePropertyChanged(nameof(AllDatas));

        }
    }
}
