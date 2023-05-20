using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using HslCommunication;
using HslCommunication.Profinet.Omron;
namespace server

{
    internal class Program
    {
        static void Main(string[] args)
        {
            //int port = 888;
            //TcpClient tcpClient = new TcpClient();
            //IPAddress[] serverIP = Dns.Ge
            //简单短连接
            //本机地址  服务器端口
            OmronFinsNet omronFinsNet = new OmronFinsNet("127.0.0.1", 9600);
            short D100 = omronFinsNet.ReadInt16("D100").Content;
            int D200 = omronFinsNet.ReadInt32("D200").Content;

            Console.WriteLine("短连接读取" + D100);
            Console.WriteLine("短连接读取" + D200);
            //Console.ReadKey();

            //简单长连接
            //OperateResult connect = omronFinsNet.ConnectServer();
            //安全操作

            //OperateResult只带有成功标准和错误信息
            OperateResult<short> readD100 = omronFinsNet.ReadInt16("D100");
            if (readD100.IsSuccess)
            {
               short value= readD100.Content;
               Console.WriteLine("长连接读取"+value);
            }
            else
            {
                Console.WriteLine("连接失败");
            }

            omronFinsNet.Write("D300",123);
            int D300 = omronFinsNet.ReadInt32("D300").Content;
            Console.WriteLine(D300);
            OperateResult<bool> rb = omronFinsNet.ReadBool("D400");
            if (rb.IsSuccess)
            {
                Console.WriteLine(rb.Content);
            }
            else
            {
                Console.WriteLine("连接失败");
            }
            byte[] buffer = new byte[8];
            // 转化为4个short
            short[] short_value = omronFinsNet.ByteTransform.TransInt16(buffer, 0, 4);
            // 转化为2个float
            float[] float_value = omronFinsNet.ByteTransform.TransSingle(buffer, 0, 2);
            // 其他的类型转换是类似的
            Console.ReadKey();
        }
    }
}
