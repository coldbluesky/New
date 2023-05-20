using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Base;

namespace Zhaoxi.AirCompression.Model
{
    public class VariableModel : ObservableObject
    {
        public Action<RecordEntity> SaveRecord;
        public Action<string> AlarmNotify;
        public Action<HistoryValueModel> VariableChanged;

        public ObservableCollection<HistoryValueModel> HistoryValues { get; set; } = new ObservableCollection<HistoryValueModel>();


        private bool _isSelected;
        /// <summary>
        /// 配合趋势曲线
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }
        public string DeivceNum { get; set; }
        /// <summary>
        /// 点位配置编号
        /// </summary>
        public string VariableNum { get; set; }
        /// <summary>
        /// 点位描述
        /// </summary>
        public string VariableDescription { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string VarAddress { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string VarType { get; set; }
        /// <summary>
        /// 读写标记
        /// </summary>
        public string OperationType { get; set; }
        /// <summary>
        /// 偏移操作符
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public string OffsetValue { get; set; }

        private string _currentValue;

        public string CurrentValue
        {
            get { return _currentValue; }
            set
            {
                HistoryValueModel hvm = new HistoryValueModel
                {
                    Value = double.Parse(value),
                    Time = DateTime.Now
                };
                HistoryValues.Add(hvm);
                if (HistoryValues.Count > 50)
                    HistoryValues.RemoveAt(0);

                VariableChanged?.Invoke(hvm);

                if (_currentValue == value) return;
                Set<string>(ref _currentValue, value);

                int record_state = 0;
                AlarmModel record_alarm = null;
                UnionModel record_union = null;
                // 报警 处理
                // 1、获取报警数据    _DAL  ->BLL  ->ViewModel
                // 2、进行报警判断
                if (this.AlarmConditions == null || this.AlarmConditions.Count == 0) return;
                foreach (var item in this.AlarmConditions)
                {
                    //  > 80
                    //  > 100
                    //  < 10
                    //  < 0
                    // 123 > 100 
                    string condition = value + item.Operator + item.CompareValue.ToString();
                    if (Boolean.TryParse(new DataTable().Compute(condition, "").ToString(), out bool state) && state)
                    {
                        // 当>判断成功后，检查所有的>
                        //var query = (from q in Alarms
                        //             where q.Operator == item.Operator && 
                        //             Boolean.TryParse(new DataTable().Compute(value + q.Operator + q.CompareValue.ToString(), "").ToString(),
                        //             out bool state) && state
                        //             select q).ToList();
                        //if (query.Count > 1)
                        //{
                        //    //if (item.Operator == "<" || item.Operator == "<=")
                        //    //    currentValue = conditionList.Min(v => v.CompareValue);
                        //    //else if (operatorStr == ">" || operatorStr == ">=")
                        //    //    currentValue = conditionList.Max(v => v.CompareValue);
                        //}

                        record_alarm = this.CompareValues<AlarmModel>(AlarmConditions, item.Operator, double.Parse(value));

                        record_state = 10;
                        // 做报警逻辑处理
                        // 做通知-》主界面、报警查询窗口
                        Messenger.Default.Send<AlarmModel>(record_alarm, "AlarmNotification");
                        this.AlarmNotify(record_alarm.AlarmDesc);
                        break;
                    }
                }

                // 联控处理
                if (this.UnionConditions == null || this.UnionConditions.Count == 0) return;
                foreach (var item in this.UnionConditions)
                {
                    string condition = value + item.Operator + item.CompareValue.ToString();
                    if (Boolean.TryParse(new DataTable().Compute(condition, "").ToString(), out bool state) && state)
                    {
                        record_union = this.CompareValues<UnionModel>(UnionConditions, item.Operator, double.Parse(value));

                        record_state = 2;
                        // 进行获取设备信息（通信配置）
                        // 获取点位信息
                        // Model   ViewModel
                        Messenger.Default.Send<UnionModel>(record_union, "UnionNotification");
                    }
                }

                // 存库
                RecordEntity recordModel = new RecordEntity
                {
                    VariableNum = this.VariableNum,
                    VariableName = this.VariableDescription,
                    RecordValue = value,
                    State = record_state,
                    AlarmNum = record_alarm?.ConditionNum,
                    UnionNum = record_union?.ConditionNum,
                    RecordTime = (DateTime.Now.Ticks - 621355968000000000) / 10000000,
                    UserId = GlobalValues.UserInfo.UserName
                };
                SaveRecord?.Invoke(recordModel);
            }
        }


        public List<AlarmModel> AlarmConditions { get; set; }
        public List<UnionModel> UnionConditions { get; set; }



        private T CompareValues<T>(List<T> conditionList, string operatorStr, double currentValue)
            where T : ConditionBaseModel
        {
            var query = (from q in conditionList
                         where q.Operator == operatorStr &&
                         Boolean.TryParse(new DataTable().Compute(currentValue + q.Operator + q.CompareValue.ToString(), "").ToString(),
                         out bool state) && state
                         select q).ToList();
            if (query.Count > 1)
            {
                if (operatorStr == "<" || operatorStr == "<=")
                    currentValue = conditionList.Min(v => v.CompareValue);
                else if (operatorStr == ">" || operatorStr == ">=")
                    currentValue = conditionList.Max(v => v.CompareValue);

                return query.FirstOrDefault(v => v.CompareValue == currentValue);
            }
            return query[0];
        }


        public void Refresh(VariableModel variableModel)
        {
            this.VariableNum = variableModel.VariableNum;
            this.VariableDescription = variableModel.VariableDescription;
            this.VarAddress = variableModel.VarAddress;
            this.VarType = variableModel.VarType;
            this.OperationType = variableModel.OperationType;
            this.Operator = variableModel.Operator;
            this.OffsetValue = variableModel.OffsetValue;
        }
    }
}
