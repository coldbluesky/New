using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Zhaoxi.DigitaPlatform.Models
{
    public class TrendAxisModel : ObservableObject
    {
        public string ANum { get; set; } = "A" + DateTime.Now.ToString("yyyyMMddHHmmssFFFF");
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                Set(ref _title, value);
                Axis.Title = IsShowTitle ? value : "";
            }
        }
        private bool _isShowTitle;

        public bool IsShowTitle
        {
            get { return _isShowTitle; }
            set
            {
                Set(ref _isShowTitle, value);
                Axis.Title = value ? Title : "";
            }
        }

        private double _minimum = 0;

        public double Minimum
        {
            get { return _minimum; }
            set
            {
                Set(ref _minimum, value);
                Axis.MinValue = value;
            }
        }

        private double _maximum = 100;

        public double Maximum
        {
            get { return _maximum; }
            set
            {
                Set(ref _maximum, value);
                Axis.MaxValue = value;
            }
        }

        private bool _isShowSeperator = false;

        public bool IsShowSeperator
        {
            get { return _isShowSeperator; }
            set
            {
                Set(ref _isShowSeperator, value);

                Axis.Separator.StrokeThickness = value ? 1 : 0;
            }
        }

        private string _labelFormater = "00";

        public string LabelFormater
        {
            get { return _labelFormater; }
            set
            {
                Set(ref _labelFormater, value);

                Axis.LabelFormatter = new Func<double, string>(v => v.ToString(value));
            }
        }

        private string _position = "Left";

        public string Position
        {
            get { return _position; }
            set
            {
                Set(ref _position, value);

                if (value == "Left")
                    this.Axis.Position = LiveCharts.AxisPosition.LeftBottom;
                else if (value == "Right")
                    this.Axis.Position = LiveCharts.AxisPosition.RightTop;
            }
        }
        public string SectionValues
        {
            get
            {
                if (Sections.Count == 0)
                    return "<未配置>";
                return string.Join(",", Sections.Select(s => s.Value)); ;
            }
        }

        private ObservableCollection<TrendSectionModel> _sections =
            new ObservableCollection<TrendSectionModel>();
        public ObservableCollection<TrendSectionModel> Sections
        {
            get => _sections;
            set
            {
                _sections = value;

                Axis.Sections.Clear();
                foreach (var item in value)
                {
                    Axis.Sections.Add(item.Section);
                }
            }
        }

        public RelayCommand AddSectionCommand { get; set; }
        public RelayCommand<TrendSectionModel> DeleteSectionCommand { get; set; }

        // LiveCharts的对象  直接对接图表的
        public Axis Axis { get; set; } = new Axis()
        {
            Title = "",
            MinValue = 0,
            MaxValue = 100,
            LabelFormatter = new Func<double, string>(v => v.ToString("00")),

            Separator = new Separator { Step = 20, Stroke = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)) },

            Sections = new SectionsCollection()
        };


        public TrendAxisModel()
        {
            AddSectionCommand = new RelayCommand(() =>
            {
                TrendSectionModel tsmodel = new TrendSectionModel();
                Sections.Add(tsmodel);
                tsmodel.ValueChanged = new Action(() =>
                {
                    this.RaisePropertyChanged(nameof(SectionValues));

                    Axis.Model.Chart.Updater.Run(false, true);
                });
                Axis.Sections.Add(tsmodel.Section);


                Axis.Model.Chart.Updater.Run(false, true);
            });
            DeleteSectionCommand = new RelayCommand<TrendSectionModel>(model =>
            {
                Sections.Remove(model);
                Axis.Sections.Remove(model.Section);
                Axis.Model.Chart.Updater.Run(false, true);
            });
        }
    }
}
