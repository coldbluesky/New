using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class SimulatorViewModel : ViewModelBase
    {
        private int _runCount = 9;

        public int RunCount
        {
            get { return _runCount; }
            set { Set(ref _runCount, value); }
        }

        public ObservableCollection<AlarmModel> Alarms { get; set; }



        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { Set(ref _selectedIndex, value); }
        }

        private object _currentDevice;

        public object CurrentDevice
        {
            get { return _currentDevice; }
            set { Set(ref _currentDevice, value); }
        }

        public string CurrentComponentId { get; set; }
        public ICommand ComponentClickCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                CurrentDevice = ((object[])obj)[0];
                this.SelectedIndex = int.Parse(((object[])obj)[1].ToString());
                this.CurrentComponentId = ((object[])obj)[2].ToString();
            });
        }


        public ICommand ChangeDeviceCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                SimpleIoc.Default.GetInstance<ChooseDeviceViewModel>().ComponentId = this.CurrentComponentId;
                SimpleIoc.Default.GetInstance<ChooseDeviceViewModel>().DeviceId = (CurrentDevice as DeviceModel)?.DeviceNum;

                Messenger.Default.Send<string>("", "ChangeDevice");
            });
        }

        IDeviceBLL _deviceBLL;
        public SimulatorViewModel(IDeviceBLL deviceBLL)
        {
            _deviceBLL = deviceBLL;
            // 获取运行状态的设备数量
            // 在运行时，有状态变化时，数量需要更新


            Messenger.Default.Register<AlarmModel>(this, "AlarmNotification", DoAlarm);
            Messenger.Default.Register<string>(this, "ChangeDeviceResult", OnChangeDeviceResult);

            // 从数据库获取当前第台设备的绑定信息
            this.AD01 = deviceBLL.GetConfig("AD01")?.ConfigDevice;
        }
        private void OnChangeDeviceResult(string data)
        {
            string cid = SimpleIoc.Default.GetInstance<ChooseDeviceViewModel>().ComponentId;
            string did = SimpleIoc.Default.GetInstance<ChooseDeviceViewModel>().DeviceId;
            // 保存配置
            if (_deviceBLL.SaveConfig(cid, did) > 0)
            {
                this.GetType().GetProperty(cid, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).SetValue(this, did);

                ViewModelLocator.Cleanup<ChooseDeviceViewModel>();
            }
        }

        private void DoAlarm(AlarmModel alarm)
        {
            Debug.WriteLine(alarm.AlarmDesc);
            this.Alarms.Add(alarm);
        }



        private string _name1;

        public string AD01
        {
            get { return _name1; }
            set { Set(ref _name1, value); }
        }

        private string _name2;

        public string AD02
        {
            get { return _name2; }
            set { Set(ref _name2, value); }
        }
    }
}
