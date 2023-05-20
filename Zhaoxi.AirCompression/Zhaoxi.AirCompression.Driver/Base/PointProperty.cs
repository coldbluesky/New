using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Base
{
    public class PointProperty
    {
        // 
        public string PointId { get; set; }
        // 基础地址 （）
        public string Address { get; set; }
        public Type ValueType { get; set; }
        public int ByteCount { get; set; }
        public byte[] ValueBytes { get; set; }
    }
}
