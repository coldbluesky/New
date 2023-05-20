using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Base
{
    internal class ModbusAddress : CommAddress
    {
        public ModbusAddress() { }
        public ModbusAddress(ModbusAddress address)
        {
            this.FuncCode = address.FuncCode;
            this.StartAddress = address.StartAddress;
            this.Length = address.Length;
        }
        public int FuncCode { get; set; }
        public int StartAddress { get; set; }
        public int Length { get; set; }
    }
}
