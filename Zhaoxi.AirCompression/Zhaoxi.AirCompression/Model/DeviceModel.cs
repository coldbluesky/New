using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Zhaoxi.AirCompression.Model
{
    // 很多信息都需要在应用中一运行的
    public class DeviceModel : ObservableObject
    {
        public DeviceModel() { }
        public DeviceModel(DeviceModel deviceModel)
        {
            this.DeviceNum = deviceModel.DeviceNum;
            this.DeviceName = deviceModel.DeviceName;
            this.CommProperties = new ObservableCollection<CommPropertyModel>(deviceModel.CommProperties);
        }
        public void Refresh(DeviceModel deviceModel)
        {
            this.DeviceName = deviceModel.DeviceName;
            this.CommProperties = new ObservableCollection<CommPropertyModel>(deviceModel.CommProperties);
        }
        public void RefreshVariable(VariableModel variableModel)
        {
            VariableModel vm = this.Variables.FirstOrDefault(v => v.VariableNum == variableModel.VariableNum);
            if (vm == null)
                this.Variables.Add(variableModel);
            else
                this.Variables.FirstOrDefault(v => v.VariableNum == variableModel.VariableNum)?.Refresh(variableModel);
        }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNum { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }   // 可能会显示在界面，需要使用通知属性？
        /// <summary>
        /// 设备状态
        /// </summary>
        private int _state = 1;

        public int State
        {
            get { return _state; }
            set { Set(ref _state, value); }
        }
        private string _alarmInfo;

        public string AlarmInfo
        {
            get { return _alarmInfo; }
            set { Set(ref _alarmInfo, value); }
        }




        private string _commErrorMessage;
        /// <summary>
        /// 设备通信异常，消息提示
        /// </summary>
        public string CommErrorMessage
        {
            get { return _commErrorMessage; }
            set { Set<string>(ref _commErrorMessage, value); }
        }

        /// <summary>
        /// 通信配置信息集合
        /// </summary>
        public ObservableCollection<CommPropertyModel> CommProperties { get; set; } = new ObservableCollection<CommPropertyModel>();
        /// <summary>
        /// 点位配置信息集合
        /// </summary>
        public ObservableCollection<VariableModel> Variables { get; set; } = new ObservableCollection<VariableModel>();

        public VariableModel this[string key]
        {
            get => Variables.FirstOrDefault(l => l.VariableNum == key);
        }

        public ICommand CancelAlarmCommand
        {
            get => new RelayCommand(() =>
            {
                this.State = 1;
            });
        }
    }
}
