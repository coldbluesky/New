using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class ChooseDeviceViewModel : ViewModelBase
    {
        public string ComponentId { get; set; }

        private string _deviceId;

        public string DeviceId
        {
            get { return _deviceId; }
            set { Set(ref _deviceId, value); }
        }

        public ICommand CloseCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                (obj as Window).DialogResult = true;
            });
        }


        public ObservableCollection<DeviceModel> Devices { get; set; }

        public ChooseDeviceViewModel(IDeviceBLL deviceBLL)
        {
            Devices = new ObservableCollection<DeviceModel>(deviceBLL.GetDeviceList());
        }
    }
}
