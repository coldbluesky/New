using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Base
{
    public enum Protocols
    {
        // 串口
        ModbusRtu = 0x01,
        ModbusAscii = 0x02,

        // 网口
        ModbusTcp = 0x81,
        S7Net = 0x82,
        FinsTcp = 0x83,
        Qna_3E = 0x84,
        A_1E = 0x85,

        ////
    }
}
