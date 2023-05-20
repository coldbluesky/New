using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class AlarmViewModel:ViewModelBase
    {
        public ObservableCollection<RecordModel> Alarms { get; set; } = new ObservableCollection<RecordModel>();


        IAlarmBLL _alarmBLL;
        public AlarmViewModel(IAlarmBLL alarmBLL)
        {
            _alarmBLL = alarmBLL;

            Refresh();

            // 注册一个报警响应
            Messenger.Default.Register<AlarmModel>(this, "AlarmNotification", OnAlarmNofitication);
        }

        private void OnAlarmNofitication(AlarmModel data)
        {
            Refresh();
        }

        private void Refresh()
        {
            var datas = _alarmBLL.GetAlarm();
            Alarms.Clear();
            datas.ForEach(d => Alarms.Add(d));
        }

        public ICommand RefreshCommand
        {
            get => new RelayCommand(Refresh);
        }

        private RelayCommand<object> _cancelAlarmCommand;
        public RelayCommand<object> CancelAlarmCommand
        {
            get
            {
                if (_cancelAlarmCommand == null)
                    _cancelAlarmCommand = new RelayCommand<object>(
                        obj =>
                        {
                            var model = obj as RecordModel;
                            if (model == null) return;

                            model.State = "1";

                            _cancelAlarmCommand.RaiseCanExecuteChanged();

                            if (Alarms.Count(a => a.DeviceNum == model.DeviceNum && a.State == "10") == 0)
                            {
                                SimpleIoc.Default.GetInstance<MainViewModel>().Devices[model.DeviceNum].State = 1;
                            }

                            // 状态保存到数据库
                            _alarmBLL.SetAlarmState(model.id.ToString(), "1");
                        },
                        obj =>
                        {
                            var model = obj as RecordModel;
                            if (model == null) return false;
                            return model.State == "10";
                        });
                return _cancelAlarmCommand;
            }
        }
    }
}
