using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Base
{
    /// <summary>
    /// 通信属性对象
    /// </summary>
    public class CommProperty
    {
        /// <summary>
        /// 通信协议，默认值 ModbusRTU
        /// </summary>
        public Protocols Protocol { get; set; } = Protocols.ModbusRtu;
        /// <summary>
        /// 字节序，默认值 ABCD
        /// </summary>
        public EndianType EndianType { get; set; } = EndianType.ABCD;

        #region 串口属性
        /// <summary>
        /// 串口名称，默认值 COM1
        /// </summary>
        /// 特性
        public string PortName { get; set; }
        /// <summary>
        /// 波特率，默认值 9600
        /// </summary>
        public string BaudRate { get; set; } = "9600";
        /// <summary>
        /// 校验位，默认值 无校验
        /// </summary>
        public Parity Parity { get; set; } = Parity.None;
        /// <summary>
        /// 数据位，默认值 8
        /// </summary>
        public string DataBits { get; set; } = "8";
        /// <summary>
        /// 停止位，默认值 1
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;
        #endregion

        #region 网口属性
        /// <summary>
        /// IP地址，默认值 127.0.0.1
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口号，默认值 502
        /// </summary>
        public string Port { get; set; }
        #endregion

        #region Modbus属性
        /// <summary>
        /// Modbus协议从站地址，默认值 1
        /// </summary>
        public string SlaveID { get; set; } = "1";
        #endregion

        #region S7属性
        /// <summary>
        /// S7协议机架号，默认值 0
        /// </summary>
        public int Rack { get; set; } = 0;
        /// <summary>
        /// S7协议插槽号，默认值 1
        /// </summary>
        public int Slot { get; set; } = 1;
        #endregion

        #region FinsTCP属性

        #endregion

        #region QnA-3E属性
        //net_code = 0x00, byte station_code = 0x00
        /// <summary>
        /// QnA-3E协议网络号，默认值 0
        /// </summary>
        public int NetCode { get; set; } = 0;
        /// <summary>
        /// QnA-3E协议站号，默认值 0
        /// </summary>
        public int StationCode { get; set; } = 0;
        #endregion


        public bool Compare(CommProperty commProperty)
        {
            if (this.PortName == commProperty.PortName ||
                (this.IP == commProperty.IP &&
                this.Port == commProperty.Port))
                return true;

            return false;
        }
    }
}
