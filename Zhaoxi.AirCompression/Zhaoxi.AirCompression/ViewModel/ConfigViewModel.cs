using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zhaoxi.AirCompression.BLL;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class ConfigViewModel
    {
        IDeviceBLL _deviceBLL;
        public ConfigViewModel(IDeviceBLL device)
        {
            _deviceBLL = device;

            Messenger.Default.Register<string>(this, "ModifyDeviceResult", OnModifyDeviceResult);
        }

        private void OnModifyDeviceResult(string data)
        {

        }
        public ICommand ModifyDeviceCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (obj == null)
                {
                    SimpleIoc.Default.GetInstance<ModifyDeviceViewModel>().Device = new Model.DeviceModel();
                }
                else
                {
                    SimpleIoc.Default.GetInstance<ModifyDeviceViewModel>().Device = new Model.DeviceModel(obj as Model.DeviceModel);
                }
                Messenger.Default.Send<string>("", "ModifyDevice");
            });
        }


        public ICommand ModifyVariableCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                SimpleIoc.Default.GetInstance<ModifyVariableViewModel>().DeviceNum = (obj as Model.DeviceModel).DeviceNum;
                SimpleIoc.Default.GetInstance<ModifyVariableViewModel>().Variable = new Model.VariableModel();

                Messenger.Default.Send<string>("", "ModifyVariable");
            });
        }
    }
}
