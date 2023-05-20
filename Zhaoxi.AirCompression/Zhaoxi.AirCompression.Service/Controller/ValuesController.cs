using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Driver;
using Zhaoxi.AirCompression.Driver.Base;
using Zhaoxi.AirCompression.Driver.Execute;

namespace Zhaoxi.AirCompression.Service.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Hello WebApi";
        }

        [HttpGet]
        [Route("monitor")]
        public ActionResult<string> GetValue()
        {
            // 通信配置信息
            Dictionary<string, object> propDic = new Dictionary<string, object>();
            propDic.Add("Protocol", Enum.Parse(typeof(Protocols), "ModbusRtu"));
            propDic.Add("PortName", "COM1");
            propDic.Add("SlaveID", "1");
            CommProperty commProperty = new CommProperty();
            // 反射
            foreach (var item in propDic)
            {
                commProperty.GetType().GetProperty(item.Key).SetValue(commProperty, item.Value);
            }

            // 监控点位信息
            List<string[]> points = new List<string[]>();
            #region Modbus Address
            points.Add(new string[] { "40001", "System.UInt16" });   // 保持寄存器 0 -1
            points.Add(new string[] { "40002", "System.UInt16" });   // 保持寄存器 1 -1
            #endregion

            // 从数据库到地址集合
            List<PointProperty> pointProps = new List<PointProperty>();
            foreach (var item in points)
            {
                Type type = Type.GetType(item[1]);// 根据类型字符串，获取对应的Type
                int typeLen = Marshal.SizeOf(type);
                pointProps.Add(new PointProperty
                {
                    Address = item[0],// 地址字符串
                    ValueType = type, // 对应的数据类型
                    ByteCount = typeLen,
                    ValueBytes = new byte[typeLen]
                });
            }

            // 
            List<int> datas = new List<int>();

            Communication communication = Communication.Create(commProperty);
            var read_result = communication.Read(pointProps);
            if (read_result.IsSuccessed)
            {
                foreach (var point in pointProps)
                {
                    datas.Add(int.Parse(communication.ConvertValue(point.ValueBytes, point.ValueType).ToString()));
                }
            }
            else
                Debug.WriteLine(read_result.Message);
            communication.Dispose();

            return Ok(datas);
        }
    }
}
