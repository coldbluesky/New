using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.DigitaPlatform.Models
{
    public class VariableModel : ObservableObject
    {
        public string DeviceNum { get; set; }
        // 唯一编码
        public string VarNum { get; set; }
        // 名称
        public string VarName { get; set; }
        // 地址
        public string VarAddress { get; set; }
        // 读取返回数据
        private object _value = 0;
        public object Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;

                Set(ref _value, value);

                //// 如果自动联动的时候
                //foreach (var ac in AlarmConditions)
                //{
                //    string exp = value.ToString() + ac.Operator + ac.CompareValue;
                //    if (bool.TryParse(dataTable.Compute(exp, "").ToString(), out bool result) && result)
                //    {
                //        // 报警条件命中
                //        // 问题：同类型条件的极值判断

                //        // <20
                //        // <10
                //        // <0
                //        // >80  --- 命中
                //        // >90  --- 希望命中

                //        // 100 实时数据
                //        // 5   
                //        // 如何解决？思路
                //        // 1、找到了命中的条件   符号
                //        // 2、所有的同符号的拿出来，再做比对
                //        // 3、小于的时候：所有小于条件满足的数据  最小获取;大于条件的时候，取最大


                //        var cm = CompareValues(ac.Operator, value);
                //        // cm就是最终的报警消息，提醒到设备
                //        // 两种方式处理：
                //        // 1、把报警逻辑写到MainViewModel中
                //        // 2、消息
                //        // 3、多处接收：设备对象、监控预警列表 
                //        Messenger.Default.Send<DeviceAlarmModel>(new DeviceAlarmModel
                //        {
                //            ANum = "A-" + DeviceNum + "-" + DateTime.Now.ToString("yyyyMMddHHmmssFFF"),
                //            CNum = cm.CNum,
                //            DNum = DeviceNum,
                //            VNum = VarNum,
                //            AlarmContent = cm.AlarmContent,
                //            AlarmValue = value.ToString(),
                //            DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //            Level = 1,
                //            State = 0
                //        }, "Alarm");

                //        break;
                //    }
                //}
            }
        }
        private ConditionModel CompareValues(string operatorStr, object currentValue)
        {
            // 查找出了所有满足指定条件的预警值
            var query = (from q in this.AlarmConditions
                         where
                            q.Operator == operatorStr &&
                            Boolean.TryParse(
                                 new DataTable().Compute(currentValue + q.Operator + q.CompareValue.ToString(), "").ToString(),
                                 out bool state) &&
                             state
                         select q).ToList();

            // 进行大小值检查
            if (query.Count > 1)
            {
                if (operatorStr == "<" || operatorStr == "<=")
                    currentValue = AlarmConditions.Min(v => double.Parse(v.CompareValue));
                else if (operatorStr == ">" || operatorStr == ">=")
                    currentValue = AlarmConditions.Max(v => double.Parse(v.CompareValue));

                return query.FirstOrDefault(v => v.CompareValue == currentValue);
            }
            return query[0];
        }

        DataTable dataTable = new DataTable();
        // 偏移量
        public double Offset { get; set; }
        // 系数
        public double Modulus { get; set; } = 1;

        public string VarType { get; set; } = "UInt16";


        public ObservableCollection<ConditionModel> AlarmConditions { get; set; } =
            new ObservableCollection<ConditionModel>();
        public ObservableCollection<ConditionModel> UnionConditions { get; set; } =
            new ObservableCollection<ConditionModel>();

        public RelayCommand AddConditionCommand { get; set; }
        public RelayCommand<object> DeleteConditionCommand { get; set; }

        public RelayCommand AddUnionCommand { get; set; }
        public RelayCommand<object> DeleteUnionCommand { get; set; }


        public VariableModel()
        {
            //AlarmConditions = new ObservableCollection<ConditionModel>
            //{
            //    new ConditionModel {CNum="123", Operator = "<", CompareValue = "123",AlarmContent="报警消息" },
            //    new ConditionModel {CNum="456", Operator = ">", CompareValue = "1230" }
            //};
            AddConditionCommand = new RelayCommand(() =>
            {
                AlarmConditions.Add(new ConditionModel() { Operator = "<", CNum = "C" + DateTime.Now.ToString("yyyyMMddHHmmssFFF") });
            });
            DeleteConditionCommand = new RelayCommand<object>(obj =>
            {
                AlarmConditions.Remove(obj as ConditionModel);
            });

            AddUnionCommand = new RelayCommand(() =>
            {
                UnionConditions.Add(new ConditionModel { Operator = "<", CNum = "C" + DateTime.Now.ToString("yyyyMMddHHmmssFFFF") });
            });
            DeleteConditionCommand = new RelayCommand<object>(obj =>
            {
                UnionConditions.Remove(obj as ConditionModel);
            });
        }
    }
}
