using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Zhaoxi.DigitaPlatform.Models
{
    public class TrendSectionModel
    {
        public Action ValueChanged { get; set; }

        private double _value = 0;

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Section.Value = value;
                ValueChanged?.Invoke();
            }
        }

        private string _color = "Red";

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;

                Section.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                ValueChanged?.Invoke();
            }
        }
        public List<string> BrushList
        {
            get
            {
                return typeof(Brushes).GetProperties().Select(p => p.Name).ToList();
            }
        }

        // 图表对象  来自在于LiveCharts
        public AxisSection Section { get; set; } = new AxisSection()
        {
            Value = 0,
            Stroke = Brushes.Red,
            StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection { 5, 5 }
        };
    }
}
