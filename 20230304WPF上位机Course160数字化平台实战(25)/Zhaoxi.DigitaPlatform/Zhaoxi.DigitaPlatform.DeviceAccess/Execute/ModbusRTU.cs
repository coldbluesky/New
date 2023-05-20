using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Zhaoxi.DigitaPlatform.Common.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Transfer;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Execute
{
    public class ModbusRTU : ModbusBase
    {
        /// <summary>
        /// 查找共用的通信对象
        /// </summary>
        /// <param name="props">参数集合   属性键值对</param>
        /// <param name="tos"></param>
        internal override Result Match(List<DevicePropItemEntity> props, List<TransferObject> tos)
        {
            var ps = props.Where(p => p.PropName == "PortName").Select(p => p.PropValue).ToList();
            return this.Match(props, tos, ps, "SerialUnit");
        }

        protected override List<byte> Read(byte slaveNum, byte funcCode, ushort startAddr, ushort count, ushort respLen)
        {
            // 一、组建请求报文
            List<byte> dataBytes = this.CreateReadPDU(slaveNum, funcCode, startAddr, count);
            // 二、计算关拼接CRC校验码
            CRC16(dataBytes);
            // 三、打开/检查通信组件的状态
            var prop = this.Props.FirstOrDefault(p => p.PropName == "TryCount");
            int tryCount = 30;
            if (prop != null)
                int.TryParse(prop.PropValue, out tryCount);
            Result connectState = this.TransferObject.Connect(tryCount);
            if (!connectState.Status)
                throw new Exception(connectState.Message);


            // 四、发送请求报文
            prop = this.Props.FirstOrDefault(p => p.PropName == "Timeout");
            int timeout = 5000;
            if (prop != null)
                int.TryParse(prop.PropValue, out timeout);
            Result<List<byte>> resp = this.TransferObject.SendAndReceived(
                dataBytes, // 发送的请求报文 
                respLen + 5,
                5, // 异常响应报文长度
                timeout);
            if (!resp.Status)
                throw new Exception(resp.Message);


            // 五、校验检查
            List<byte> crcValidation = resp.Data.GetRange(0, resp.Data.Count - 2);
            this.CRC16(crcValidation);
            if (!crcValidation.SequenceEqual(resp.Data))
            {
                throw new Exception("CRC校验检查不匹配");
                // CRC 校验失败
            }

            // 六、检查异常报文
            if (resp.Data[1] > 0x80)
            {
                // 
                byte errorCode = resp.Data[2];
                throw new Exception(Errors[errorCode]);
            }
            // 七、解析
            List<byte> datas = resp.Data.GetRange(3, resp.Data.Count - 5);
            return datas;
        }

        protected override void Write(byte slaveNum, byte funcCode, ushort startAddr, byte[] data, ushort respLen)
        {
            // 一、组建请求报文
            List<byte> dataBytes = this.CreateWritePDU(slaveNum, funcCode, startAddr, data);
            // 二、计算关拼接CRC校验码
            CRC16(dataBytes);
            // 三、打开/检查通信组件的状态
            var prop = this.Props.FirstOrDefault(p => p.PropName == "TryCount");
            int tryCount = 30;
            if (prop != null)
                int.TryParse(prop.PropValue, out tryCount);
            Result connectState = this.TransferObject.Connect(tryCount);
            if (!connectState.Status)
                throw new Exception(connectState.Message);


            // 四、发送请求报文
            prop = this.Props.FirstOrDefault(p => p.PropName == "Timeout");
            int timeout = 5000;
            if (prop != null)
                int.TryParse(prop.PropValue, out timeout);


            Result<List<byte>> resp = this.TransferObject.SendAndReceived(
                dataBytes, // 发送的请求报文 
                8,
                5,// 异常响应报文长度
                timeout);
            if (!resp.Status)
                throw new Exception(resp.Message);


            // 五、校验检查
            List<byte> crcValidation = resp.Data.GetRange(0, resp.Data.Count - 2);
            this.CRC16(crcValidation);
            if (!crcValidation.SequenceEqual(resp.Data))
            {
                throw new Exception("CRC校验检查不匹配");
                // CRC 校验失败
            }

            // 六、检查异常报文
            if (resp.Data[1] > 0x80)
            {
                byte errorCode = resp.Data[2];
                throw new Exception(Errors.ContainsKey(errorCode) ? Errors[errorCode] : "自定义异常");
            }
        }


        /// <summary>
        /// 计算CRC校验码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="poly"></param>
        /// <param name="crcInit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        void CRC16(List<byte> value, ushort poly = 0xA001, ushort crcInit = 0xFFFF)
        {
            if (value == null || !value.Any())
                throw new ArgumentException("");

            //运算
            ushort crc = crcInit;
            for (int i = 0; i < value.Count; i++)
            {
                crc = (ushort)(crc ^ (value[i]));
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ poly) : (ushort)(crc >> 1);
                }
            }
            byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
            byte lo = (byte)(crc & 0x00FF);         //低位置

            value.Add(lo);
            value.Add(hi);
        }

    }
}
