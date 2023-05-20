using System;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver.Component
{
    public interface IComponent
    {
        // 连接
        Task<Result> Connect(int timeout = 2000);

        // 关闭
        /// 通信的过程，短连接/长连接
        /// 短连接（）      读完就关  
        /// 长连接（线程）    CPU还可以、效率         
        /// 
        Result Close();


        // 
        /// <summary>
        /// 发送（字节数组）/接收（字节数组）
        /// </summary>
        /// <param name="command"></param>
        /// <param name="receiveLen">Modbus(读取：    其他协议：先读一部分，长度取剩余部分</param>
        /// <param name="errorByteLe"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<Result<byte>> SendAndReceive(byte[] command, int receiveLen, int errorByteLen, int timeout);
        void SendAndReceiveAsync(CommandTask serialTask, bool isRead = true);
    }
}
