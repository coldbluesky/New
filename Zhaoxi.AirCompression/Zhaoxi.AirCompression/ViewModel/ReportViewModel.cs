using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class ReportViewModel
    {
        /// <summary>
        ///  表示当前DataGrid中的列
        /// </summary>
        public ObservableCollection<DataGridColumn> Columns { get; set; } = new ObservableCollection<DataGridColumn>();
        /// <summary>
        /// 所有数据（做导出的时候需要取数据）
        /// </summary>
        public ObservableCollection<RecordModel> AllDatas { get; set; } = new ObservableCollection<RecordModel>();
        /// <summary>
        /// 所有的可选择列
        /// </summary>
        public ObservableCollection<DataGridColumnModel> AllColumms { get; set; } = new ObservableCollection<DataGridColumnModel>();

        IReportBLL _reportBLL;
        public ReportViewModel(IReportBLL reportBLL)
        {
            _reportBLL = reportBLL;

            // 初始化列列表
            AllColumms.Add(new DataGridColumnModel { Header = "设备编号", BindingPath = "DeviceNum", ColumnWidth = 70 });
            AllColumms.Add(new DataGridColumnModel { IsSelected = true, Header = "设备名称", BindingPath = "DeviceName", ColumnWidth = 150 });
            AllColumms.Add(new DataGridColumnModel { Header = "点位编号", BindingPath = "VariableNum", ColumnWidth = 70 });
            AllColumms.Add(new DataGridColumnModel { Header = "点位描述", BindingPath = "VariableDesc", ColumnWidth = 150 });
            AllColumms.Add(new DataGridColumnModel { IsSelected = true, Header = "记录值", BindingPath = "RecordValue", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "状态", BindingPath = "State", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "记录时间", BindingPath = "DataTime", ColumnWidth = 150 });
            AllColumms.Add(new DataGridColumnModel { Header = "记录用户", BindingPath = "UserName", ColumnWidth = 90 });
            AllColumms.Add(new DataGridColumnModel { Header = "报警描述", BindingPath = "AlarmDesc", ColumnWidth = 150 });
            AllColumms.Add(new DataGridColumnModel { Header = "报警详情", BindingPath = "AlarmDetail", ColumnWidth = 200 });
            AllColumms.Add(new DataGridColumnModel { Header = "报警等级", BindingPath = "AlarmLevel", ColumnWidth = 80 });

            // 添加默认列
            foreach (var item in AllColumms)
                ChangedColumn(item);


            // 获取所有数据
            this.Refresh();
        }

        private void Refresh()
        {
            // 添加查询条件

            var datas = _reportBLL.GetRecords();
            AllDatas.Clear();
            datas.ForEach(d => AllDatas.Add(d));
        }
        public ICommand RefreshCommand
        {
            get => new RelayCommand(() =>
            {
                Refresh();
            });
        }

        public ICommand ExportCommand
        {
            get => new RelayCommand(() =>
            {
                // 查找出所有选中的列
                // 通过Index排序，根据先选后选的顺序进行值的导出
                List<DataGridColumnModel> cs = AllColumms.Where(c => c.IsSelected).OrderBy(c => c.Index).ToList();
                // 拼接列头标签
                string datas = string.Join(",", cs.Select(c => c.Header)) + "\r\n";
                foreach (var item in AllDatas)
                {
                    foreach (var col in cs)
                    {
                        // 通过反射获取列对应的数据
                        PropertyInfo pi = item.GetType().GetProperty(col.BindingPath, BindingFlags.Instance | BindingFlags.Public);

                        var v = pi.GetValue(item);
                        datas += v == null ? "," : v.ToString() + ",";
                        datas.Remove(datas.Length - 1);
                    }
                    datas += "\r\n";
                }

                System.IO.File.WriteAllText("data.csv", datas, Encoding.UTF8);
            });
        }



        int index = 0;
        private void ChangedColumn(DataGridColumnModel model)
        {
            if (model.IsSelected)
            {
                model.Index = index++;
                Columns.Add(new DataGridTextColumn
                {
                    Header = model.Header,
                    Binding = new Binding(model.BindingPath),
                    MinWidth = model.ColumnWidth
                });
            }
            else
            {
                var column = Columns.FirstOrDefault(c => c.Header.ToString() == model.Header);
                Columns.Remove(column);
            }
        }

        public ICommand ChooseColumnCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                var model = obj as DataGridColumnModel;
                if (model.IsSelected)
                {
                    model.Index = index++;
                    Columns.Add(new DataGridTextColumn
                    {
                        Header = model.Header,
                        Binding = new Binding(model.BindingPath),
                        MinWidth = model.ColumnWidth
                    }); ;
                }
                else
                {
                    var column = Columns.FirstOrDefault(c => c.Header.ToString() == model.Header);
                    Columns.Remove(column);
                }
            });
        }
    }
}
