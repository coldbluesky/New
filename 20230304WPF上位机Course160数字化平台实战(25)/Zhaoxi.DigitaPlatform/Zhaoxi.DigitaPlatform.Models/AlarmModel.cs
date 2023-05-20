using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.IDataAccess;

namespace Zhaoxi.DigitaPlatform.Models
{
    public class AlarmModel : ObservableObject
    {
        public int Index { get; set; }
        public string id { get; set; }
        public string DeviceNum { get; set; }
        public string DeviceName { get; set; }
        public string VariableNum { get; set; }
        public string VariableName { get; set; }
        public string RecordValue { get; set; }
        private string _state;

        public string State
        {
            get { return _state; }
            set
            {
                Set(ref _state, value);
                if (value == "0")
                    StateName = "未处理";
                else if (value == "1")
                    StateName = "已处理";
                this.RaisePropertyChanged("StateName");
            }
        }
        public string StateName { get; set; }

        public string AlarmNum { get; set; }
        public string DataTime { get; set; }
        private string _solveTime;

        public string SolveTime
        {
            get { return _solveTime; }
            set { Set(ref _solveTime, value); }
        }

        public string AlarmContent { get; set; }
        public string AlarmLevel { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public RelayCommand CancelAlarmCommand { get; set; }

        ILocalDataAccess _localDataAccess;
        public AlarmModel(ILocalDataAccess localDataAccess)
        {
            _localDataAccess = localDataAccess;

            CancelAlarmCommand = new RelayCommand(DoCancelAlarm, DoCanCancelAlarm);
        }

        private void DoCancelAlarm()
        {
            this.State = "1";
            CancelAlarmCommand.RaiseCanExecuteChanged();

            // 状态保存到数据库
            this.SolveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _localDataAccess.UpdateAlarmState(this.id, this.SolveTime);

            // 通知   事件聚合器
            Messenger.Default.Send(this.DeviceNum, "CancelAlarm");
        }

        private bool DoCanCancelAlarm()
        {
            return this.State == "0";
        }
    }
}
