using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.Digital.Mobile.Models
{
    public class DeviceItemModel
    {
        // 设备图像
        public string DeviceImage { get; set; }
        // 设备名称
        public string DeviceName { get; set; }
        // 设备相关属性（监控点位）
        public List<DevicePropertyItemModel> Properties { get; set; } = new List<DevicePropertyItemModel>();
    }

    public class DevicePropertyItemModel : INotifyPropertyChanged
    {
        public int PropId { get; set; }
        public string PropName { get; set; }

        private string _propValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public string PropValue
        {
            get { return _propValue; }
            set
            {
                _propValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PropValue"));
            }
        }
    }
}
