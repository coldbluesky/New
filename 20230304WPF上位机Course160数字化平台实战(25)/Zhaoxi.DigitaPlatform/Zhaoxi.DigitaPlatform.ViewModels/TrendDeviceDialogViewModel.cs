using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.Models;

namespace Zhaoxi.DigitaPlatform.ViewModels
{
    internal class TrendDeviceDialogViewModel
    {
        public TrendModel Trend { get; set; }
        public List<TrendDeviceModel> ChooseDevicesList { get; set; }
        public List<string> BrushList { get; set; }

        public TrendDeviceDialogViewModel(TrendModel trend, List<string> brushList, MainViewModel model)
        {
            Trend = trend;
            BrushList = brushList;


            ChooseDevicesList = model.DeviceList
                .Where(d => d.VariableList.Count > 0)
                .Select(d => new TrendDeviceModel
                {
                    Header = d.Header,

                    VarList = d.VariableList.Select(v => InitDevices(d, v)).ToList()

                }).ToList();
        }

        private TrendDeviceVarModel InitDevices(DeviceItemModel dm, VariableModel vm)
        {
            // 检查一下已选择的变量有哪些
            var item = Trend.Series.ToList().FirstOrDefault(ts => ts.DNum == dm.DeviceNum && ts.VNum == vm.VarNum);

            TrendDeviceVarModel varModel = new TrendDeviceVarModel();

            varModel.IsSelected = item != null;
            varModel.DNum = dm.DeviceNum;
            varModel.VNum = vm.VarNum;
            varModel.VarName = vm.VarName;
            varModel.VarType = vm.VarType;

            varModel.AxisNum = item == null ? Trend.AxisList[0].ANum : item.AxisNum;
            varModel.Color = item == null ? BrushList[new Random().Next(0, BrushList.Count)] : item.Color;

            //deviceModel.SelectedChanged = SeriesChanged;
            varModel.PropertyChanged += (se, ev) =>
            {
                if (ev.PropertyName == "IsSelected")
                {
                    SeriesChanged(se as TrendDeviceVarModel);
                }
                else if (ev.PropertyName == "Color" || ev.PropertyName == "AxisNum")
                {
                    var m = se as TrendDeviceVarModel;
                    if (m == null) return;
                    var si = Trend.Series.FirstOrDefault(ts => ts.DNum == m.DNum && ts.VNum == m.VNum);
                    if (si == null) return;

                    si.Color = m.Color;
                    si.AxisNum = m.AxisNum;
                }
            };

            return varModel;
        }

        private void SeriesChanged(TrendDeviceVarModel model)
        {
            if (Trend == null) return;
            if (model.IsSelected)
            {
                // 添加序列
                TrendSeriesModel tsModel = new TrendSeriesModel
                {
                    DNum = model.DNum,
                    VNum = model.VNum,

                    Title = model.VarName,
                    Color = model.Color,
                    AxisIndexFunc = num => Trend.AxisList.ToList().FindIndex(a => a.ANum == num)
                };
                Trend.Series.Add(tsModel);
                Trend.ChartSeries.Add(tsModel.Series);
            }
            else
            {
                // 移除序列
                int index = Trend.Series.ToList().FindIndex(s => s.DNum == model.DNum && s.VNum == model.VNum);
                Trend.Series.RemoveAt(index);
                Trend.ChartSeries.RemoveAt(index);
            }
        }
    }
}
