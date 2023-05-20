using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zhaoxi.AirCompression.Model;
using LiveCharts.Defaults;
using System.Windows.Media;
using System.Windows;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class TrendViewModel
    {
        public SeriesCollection Series { get; set; } = new SeriesCollection();
        public AxesCollection AxisX { get; set; } = new AxesCollection();
        public AxesCollection AxisY { get; set; } = new AxesCollection();

        public ICommand AddAxisYCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                // 打开编辑窗口
                Messenger.Default.Send<string>("", "AddAxisY");
            });
        }

        public ICommand ChoosePointCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                var model = obj as VariableModel; //点位信息
                if (model == null) return;

                AddSeries(model);
            });
        }

        private void AddSeries(VariableModel model)
        {
            string key = model.VariableDescription + "-" + model.DeivceNum + "-" + model.VariableNum;
            if (model.IsSelected)
            {
                // ScalesYAt : 对齐到哪个Y轴
                LineSeries lineSeries = new LineSeries() { ScalesYAt = 0, PointGeometrySize = 0 };
                lineSeries.Title = key;
                ChartValues<ObservableValue> values = new ChartValues<ObservableValue>();

                Axis axis = new Axis()
                {
                    MaxValue = 50,
                    LabelsRotation = -45,
                    Separator = new Separator { Step = 2, IsEnabled = false },
                };
                axis.Sections = new SectionsCollection { new AxisSection { Stroke = Brushes.White, StrokeThickness = 1, Value = 0 } };
                IList<string> labels = new List<string>();
                foreach (var item in model.HistoryValues)
                {
                    values.Add(new ObservableValue(item.Value));
                    labels.Add(item.Time.ToString("HH:mm:ss"));
                    axis.Labels = labels;
                }
                model.VariableChanged = new Action<HistoryValueModel>(history =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        values.Add(new ObservableValue(history.Value));
                        labels.Add(history.Time.ToString("HH:mm:ss"));
                        if (values.Count > 50)
                        {
                            values.RemoveAt(0);
                            labels.RemoveAt(0);
                        }
                        lineSeries.Values = values;
                        axis.Labels = labels;
                    });
                });
                Series.Add(lineSeries);
                AxisX.Clear();
                AxisX.Add(axis);
            }
            else
            {
                model.VariableChanged = null;
                //移除
                var series = Series.ToList().FirstOrDefault(s => (s as LineSeries).Title.ToString() == key);
                if (series != null)
                    Series.Remove(series);
            }
        }


        public List<AxisYModel> AxisYs { get; set; } = new List<AxisYModel>();

        public void AddAxisY(AxisYModel axisYModel)
        {
            AxisYs.Add(axisYModel);

            //AxisYs.FindIndex(a => a.Title == "");

            AxisY.Add(new Axis
            {
                Title = axisYModel.Title,
                Position = AxisPosition.RightTop,
                LabelFormatter = axisYModel.IsPercent ? (val => val.ToString("N") + " %") : null,
                MinValue = 0,
                MaxValue = 100,
                Separator = new Separator { IsEnabled = false },
                Sections = new SectionsCollection { new AxisSection { Value = 0 } }
            });
        }
    }
}
