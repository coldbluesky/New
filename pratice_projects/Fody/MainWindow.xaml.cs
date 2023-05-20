using Newtonsoft.Json;
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
using System.Windows.Threading;


namespace Fody
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            GJson();
        }
        public void GJson()
        {
            List<Model> modelList= new List<Model>();
           
            modelList.Add(new Model { OperationName= "SetTemp",OperationDesc= "设定温度",OperationValue=0 });
            modelList.Add(new Model { OperationName= "TemperatureTolerance", OperationDesc= "温度公差", OperationValue=0 });
            modelList.Add(new Model { OperationName= "TemperatureLimit", OperationDesc= "温度上限", OperationValue=0 });
            modelList.Add(new Model { OperationName= "VacuumArriveValue", OperationDesc= "真空到达值", OperationValue=0 });
            modelList.Add(new Model { OperationName= "VacuumUpValue", OperationDesc= "真空上限", OperationValue=0 });
            modelList.Add(new Model { OperationName= "VacuumDownValue", OperationDesc= "真空下限", OperationValue=0 });
            modelList.Add(new Model { OperationName= "NitrogenArriveValue", OperationDesc= "氮气到达值", OperationValue=0 });
            modelList.Add(new Model { OperationName= "NitrogenUpValue", OperationDesc= "氮气上限", OperationValue=0 });
            modelList.Add(new Model { OperationName= "NitrogenDownValue", OperationDesc= "氮气下限", OperationValue=0 });
            modelList.Add(new Model { OperationName= "AtmosphericArriveValue", OperationDesc= "常压到达值", OperationValue=0 });
            modelList.Add(new Model { OperationName= "AtmosphericUpValue", OperationDesc= "常压上限", OperationValue=0 });
            modelList.Add(new Model { OperationName= "AtmosphericDownValue", OperationDesc= "常压下限", OperationValue=0 });
            for(int i = 1; i <= 4; i++)
            {
                modelList.Add(new Model { OperationName = $"CycleStartStep{i}", OperationDesc = $"循环启动工步{i}", OperationValue = 0 });
            }
            for (int i = 1; i <= 4; i++)
            {
                modelList.Add(new Model { OperationName = $"CycleEndStep{i}", OperationDesc = $"循环结束工步{i}", OperationValue = 0 });
            }
            for (int i = 1; i <= 4; i++)
            {
                modelList.Add(new Model { OperationName = $"CycleNumber{i}", OperationDesc = $"循环次数{i}", OperationValue = 0 });
            }
            for (int i = 1; i <= 20; i++)
            {
                modelList.Add(new Model { OperationName = $"HeatingEnabled{i}", OperationDesc = $"加热{i}启用", OperationValue = 0 });
            }
            for (int i = 1; i <= 20; i++)
            {
                modelList.Add(new Model { OperationName = $"VacuumEnabled{i}", OperationDesc = $"真空{i}启用", OperationValue = 0 });
            }
            for (int i = 1; i <= 20; i++)
            {
                modelList.Add(new Model { OperationName = $"NitrogenEnabled{i}", OperationDesc = $"氮气{i}启用", OperationValue = 0 });
            }
            for (int i = 1; i <= 20; i++)
            {
                modelList.Add(new Model { OperationName = $"StepWorkTime{i}", OperationDesc = $"工步{i}时间", OperationValue = 0 });
            }
            string json = JsonConvert.SerializeObject(modelList);
        }

    }
    public class Model
    {
        public String OperationName { get; set; }
        public String OperationDesc { get; set; }
        public int OperationValue { get; set; }

    }

}


