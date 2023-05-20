using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Base
{
    public class VariableProperty
    {
        public string VarId { get; set; }
        public string VarAddr { get; set; }
        public Type ValueType { get; set; }// 
        public byte[] ValueBytes { get; set; }
    }
}
