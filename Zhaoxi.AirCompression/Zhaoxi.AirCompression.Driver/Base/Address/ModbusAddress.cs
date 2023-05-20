using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Base.Address
{
    public class ModbusAddress
    {
        public ModbusAddress() { }
        public ModbusAddress(ModbusAddress modbusAddress)
        {
            this.FuncCode = modbusAddress.FuncCode;
            this.StartAddress = modbusAddress.StartAddress;
            this.Length = modbusAddress.Length;
        }
        public int FuncCode { get; set; }
        public int StartAddress { get; set; }
        public int Length { get; set; }

        public int BitAddress { get; set; } = -1;
    }
}
