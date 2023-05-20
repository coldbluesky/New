using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class RecordModel : ObservableObject
    {
        public int id { get; set; }
        public string DeviceNum { get; set; }
        public string DeviceName { get; set; }
        public string VariableNum { get; set; }
        public string VariableDesc { get; set; }
        public string RecordValue { get; set; }
        private string _state;

        public string State
        {
            get { return _state; }
            set { Set(ref _state, value); }
        }

        public string DataTime { get; set; }
        public string UserName { get; set; }
        public string AlarmNum { get; set; }
        public string AlarmDesc { get; set; }
        public string AlarmDetail { get; set; }
        public string AlarmLevel { get; set; }
    }
}
