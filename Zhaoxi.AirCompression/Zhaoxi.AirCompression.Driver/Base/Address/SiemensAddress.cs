using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Base.Address
{
    public class SiemensAddress
    {
        // Parameter类型
        // Data 类型
        // byte 
        // M10   float   4B    2W   1D
        //       short   2B    1W
        // I0.0  bool    1Byte    Bit
        // KepServer


        // 1、组合 Param-Item   一个Item包含多个地址，限制每个Item的区间长度：32个字节
        /// 两个层面：
        /// DB1.10    short        第一个Item
        /// DB1.12    short
        /// DB1.18    float
        /// DB1.50    short        第二个Item
        /// DB1.56    short
        /// DB1.58.1  bool
        /// DB1.58.2  bool
        /// M10       short        第三个Item
        /// M12       short
        /// I0        byte         第四个Item
        /// I0.0      bool
        /// I0.1      bool
        /// I1        byte
        // 2、一个请求，包含多个Param-Item[至少3个地址区间]

        // 从字符串向协议地址的转换


        public SiemensAddress() { }
        public SiemensAddress(SiemensAddress sa)
        {
            this.AreaType = sa.AreaType;
            this.DBNumber = sa.DBNumber;
            this.ByteAddress = sa.ByteAddress;
            this.BitAddress = sa.BitAddress;
            this.ByteCount = sa.ByteCount;
        }
        //public ParameterItemType ParamItemType { get; set; } = ParameterItemType.BYTE;
        //public DataItemType DataItemType { get; set; } = DataItemType.BWD;
        /// <summary>
        /// 数据访问区
        /// </summary>
        public SiemensAreaTypes AreaType { get; set; }
        public int DBNumber { get; set; } = 0;
        /// <summary>
        /// 地址分为两部分（Byte   Bit）
        /// </summary>
        public int ByteAddress { get; set; }
        public byte BitAddress { get; set; } = 0;
        /// <summary>
        /// 需要的字节个数
        /// </summary>
        public int ByteCount { get; set; }


        //public int State { get; set; } = 0xff;
        //public string ErrorMessage { get; set; }
    }
}
