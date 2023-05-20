using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.BLL
{
    public interface IDeviceBLL
    {
        List<DeviceModel> GetDevice();
        List<DeviceModel> GetDeviceList();
        DeviceModel GetDeviceAndComm(string deviceNum);
        VariableModel GetVariableById(string varNum);

        ConfigModel GetConfig(string key);
        int SaveConfig(string key, string device, string variable = "");
        int SaveDevice(DeviceModel deviceModel);
        int SaveVariable(string deviceNum, VariableModel variableModel);
    }
}
