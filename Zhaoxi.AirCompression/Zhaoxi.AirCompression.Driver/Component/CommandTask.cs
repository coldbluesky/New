using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Component
{
    public class CommandTask
    {
        public byte[] Command { get; set; }
        public int ReceiveLen { get; set; }
        public int ErrorLen { get; set; }
        public int Timeout { get; set; }
        public Action<Result<byte>> Callback { get; set; }
    }
}
