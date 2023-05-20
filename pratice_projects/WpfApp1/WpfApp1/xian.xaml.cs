using LiveCharts.Wpf;
using LiveCharts;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// xian.xaml 的交互逻辑
    /// </summary>
    public partial class xian : UserControl
    {

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Label { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public ChartValues<int> chartValues { get; set; }
        bool a = false;
        public xian()
        {
            InitializeComponent();
            GenerateValues();
            if (a)
            {
                GenerateColunm();

            }
            else
            {
                GenerateLine();
            }
            YFormatter = value => value.ToString("N");
            DataContext = this;
        }

        private ChartValues<int> GenerateValues()
        {
            Random random = new Random();
            chartValues = new ChartValues<int>() {1,2};
            Label = new List<string>() {"15:22:32", "16:50:32", "17:33:32", "18:50:32", "19:44:32" };
            //for (int i = 0; i < 500; i++)
            //{
            //    int a = random.Next(100);
            //    chartValues.Add(a);
            //    Label.Add(a.ToString());
            //}
            return chartValues;
        }
        private void GenerateColunm()
        {
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Series 1",
                    Values = chartValues,
                },
            };
        }
        private void GenerateLine()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = chartValues,
                },

                
            };
            SeriesCollection.Add(new LineSeries
            {
                Title = "Series 2",
                Values = new ChartValues<int>() { 2, 6},
            });
        }

    }
}
