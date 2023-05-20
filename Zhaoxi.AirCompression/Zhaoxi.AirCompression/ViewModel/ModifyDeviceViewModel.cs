using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class ModifyDeviceViewModel
    {
        public string ModifyType { get; set; }

        public List<ComPropertyItemModel> Properties { get; set; } = new List<ComPropertyItemModel>();
        public DeviceModel Device { get; set; } = new DeviceModel();

        public ICommand ConfirmCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                // 保存数据
                if (_deviceBLL.SaveDevice(Device) > 0)
                {
                    SimpleIoc.Default.GetInstance<MainViewModel>().Devices.Get(Device.DeviceNum)?.Refresh(Device);
                    (obj as Window).DialogResult = true;
                }
            });
        }

        public ICommand AddPropCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                if (Device.CommProperties == null)
                    Device.CommProperties = new System.Collections.ObjectModel.ObservableCollection<CommPropertyModel>();
                Device.CommProperties.Add(new CommPropertyModel
                {
                    PropName = "Protocol",
                    PropValue = "ModbusRtu"
                });
            });
        }

        IDeviceBLL _deviceBLL;
        public ModifyDeviceViewModel(IDeviceBLL deviceBLL)
        {
            _deviceBLL = deviceBLL;

            #region 初始化通信属性列表
            Properties.Add(new ComPropertyItemModel
            {
                PropKey = "Protocol",
                PropName = "通信协议",
                SelectValues = new List<string>
                {
                    "ModbusRtu","ModbusAscii","ModbusTcp","S7"
                }
            });
            Properties.Add(new ComPropertyItemModel { PropKey = "EndianType", PropName = "字节序" });
            Properties.Add(new ComPropertyItemModel
            {
                PropKey = "PortName",
                PropName = "串口名称",
                SelectValues = new List<string>(SerialPort.GetPortNames())
            }); ;
            Properties.Add(new ComPropertyItemModel { PropKey = "BaudRate", PropName = "波特率" });
            Properties.Add(new ComPropertyItemModel { PropKey = "Parity", PropName = "校验位" });
            Properties.Add(new ComPropertyItemModel { PropKey = "DataBits", PropName = "数据位" });
            Properties.Add(new ComPropertyItemModel { PropKey = "StopBits", PropName = "停止位" });
            Properties.Add(new ComPropertyItemModel { PropKey = "IP", PropName = "IP地址" });
            Properties.Add(new ComPropertyItemModel { PropKey = "Port", PropName = "端口号" });
            Properties.Add(new ComPropertyItemModel { PropKey = "SlaveID", PropName = "从站地址" });
            Properties.Add(new ComPropertyItemModel { PropKey = "Rack", PropName = "机架号" });
            Properties.Add(new ComPropertyItemModel { PropKey = "Slot", PropName = "插槽号" });
            #endregion
        }
    }
}