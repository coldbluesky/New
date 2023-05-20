using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.DAL
{
    public interface IDeviceDataAccess
    {
        // 之前使用数据实体的目的是需要从接口返回的Json字符串反序列化
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        DataTable GetDevice();
        // 添加获取指定设备的信息（联控的时候）
        DataTable GetDeviceById(string deviceNum);




        /// <summary>
        /// 通信设备编号获取设备通信参数
        /// </summary>
        /// <param name="deviceNum">设备编号</param>
        /// <returns></returns>
        DataTable GetCommProperties(string deviceNum);
        /// <summary>
        /// 通信设备编号获取点位信息
        /// </summary>
        /// <param name="deviceNum">设备编号</param>
        /// <returns></returns>
        DataTable GetVariables(string deviceNum);
        DataTable GetVariableById(string variableNum);


        DataTable GetAlarms(string varNum);
        DataTable GetUnions(string varNum);

        int Record(List<RecordEntity> records);


        DataTable GetConfig(string id);
        int SaveConfig(string id, string device, string variable);

        int SaveDevice(DeviceModel deviceModel);
        int SaveVariable(string deviceNum, VariableModel variableModel);

        void Dispose();
    }
}
