using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.DigitalService
{
    public class ClientModel
    {
        public ushort ID { get; set; }
        public Socket Client { get; set; }

        public List<string[]> Properties { get; set; } = new List<string[]>();
        public List<string[]> Variables { get; set; } = new List<string[]>();

        public DateTime Lifetime { get; set; }// 服务接收客户   Receive()


        public Dictionary<string, byte[]> Values { get; set; } = new Dictionary<string, byte[]>();
    }
}
