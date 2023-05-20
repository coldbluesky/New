using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Driver.Base;

namespace Zhaoxi.AirCompression.Driver.Component
{
    public class ComponentSocket : IComponent
    {
        private Socket socket = null;
        private ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        private bool connectState = false;
        private Stopwatch stopwatch = new Stopwatch();


        private List<CommandTask> readCommandTasks = new List<CommandTask>();
        private List<CommandTask> writeCommandTasks = new List<CommandTask>();
        private CancellationTokenSource cancelToken = new CancellationTokenSource();


        private CommProperty _commProperty;
        private Task task = null;
        public ComponentSocket(CommProperty commProperty)
        {
            _commProperty = commProperty;
            //this.ExecuteTasks();
        }

        private void ExecuteTasks()
        {
            task = Task.Run(new Action(async () =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    cancelToken.Token.ThrowIfCancellationRequested();

                    if (writeCommandTasks.Count > 0)
                    {
                        Debug.WriteLine("----写请求：" + string.Join(" ", writeCommandTasks[0].Command.Select(c => c.ToString("X2")).ToList()));
                        // 执行
                        var result = await this.SendAndReceive(writeCommandTasks[0].Command, writeCommandTasks[0].ReceiveLen, writeCommandTasks[0].ErrorLen, writeCommandTasks[0].Timeout);

                        writeCommandTasks[0].Callback?.Invoke(result);

                        if (writeCommandTasks.Count > 0)
                            writeCommandTasks.RemoveAt(0);
                    }
                    else if (readCommandTasks.Count > 0)
                    {
                        Debug.WriteLine("=====读请求：" + string.Join(" ", readCommandTasks[0].Command.Select(c => c.ToString("X2")).ToList()));
                        // 执行
                        var result = await this.SendAndReceive(readCommandTasks[0].Command, readCommandTasks[0].ReceiveLen, readCommandTasks[0].ErrorLen, readCommandTasks[0].Timeout);

                        readCommandTasks[0].Callback?.Invoke(result);

                        if (readCommandTasks.Count > 0)
                            readCommandTasks.RemoveAt(0);
                    }
                }
                Debug.WriteLine("2----------------");
                cancelToken.Cancel();
            }), cancelToken.Token);
        }
        public Result Close()
        {
            try
            {
                if (socket?.Connected ?? false) socket?.Shutdown(SocketShutdown.Both);//正常关闭连接
            }
            catch { }

            try
            {
                socket?.Close();
                return new Result();
            }
            catch (Exception ex) { return new Result(false, ex.Message); }
        }

        public async Task<Result> Connect(int timeout = 2000)
        {
            Result result = new Result();
            try
            {
                if (socket == null)
                    // ProtocolType 可支持配置
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                TimeoutObject.Reset();
                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout)
                {
                    if (!(!socket.Connected || (socket.Poll(200, SelectMode.SelectRead) && (socket.Available == 0))))
                    {
                        return result;
                    }
                    try
                    {
                        socket?.Close();
                        socket.Dispose();
                        socket = null;

                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.BeginConnect(_commProperty.IP, int.Parse(_commProperty.Port), callback =>
                        {
                            connectState = false;
                            var cbSocket = callback.AsyncState as Socket;
                            if (cbSocket != null)
                            {
                                connectState = cbSocket.Connected;

                                if (cbSocket.Connected)
                                    cbSocket.EndConnect(callback);

                            }
                            TimeoutObject.Set();
                        }, socket);
                        TimeoutObject.WaitOne(2000, false);
                        if (!connectState) throw new Exception();
                        else break;
                    }
                    catch (SocketException ex)
                    {
                        if (ex.ErrorCode == 10060)
                            throw new Exception(ex.Message);
                    }
                    catch (Exception) { }
                }
                if (socket == null || !socket.Connected || ((socket.Poll(200, SelectMode.SelectRead) && (socket.Available == 0))))
                {
                    throw new Exception("网络连接失败");
                }
            }
            catch (Exception ex)
            {
                result = new Result(false, ex.Message);
            }
            return result;
        }

        // 根据字节长度信息进行第二次的数据获取
        // Modbus是否可以？是可以的     4-5个字节
        // S7                           2-3个字节
        // FinsTcp                      4个字节
        // 字节序？需要考虑
        public async Task<Result<byte>> SendAndReceive(byte[] command, int receiveLen, int errorByteLen, int timeout)
        {
            Result<byte> result = new Result<byte>();
            try
            {
                if (command != null)
                    socket.Send(command.ToArray(), 0, command.Length, SocketFlags.None);


                int bufferSize = 512;// 字节长度的处理
                byte[] receiveBytes = new byte[receiveLen];// 定义一个正常报文长度的数组
                int receiveByteCount = 0;// 当前已经接收到的字节数

                int flag = 0;
                while (receiveByteCount < receiveLen && receiveByteCount != errorByteLen)
                {
                    // 分批读取
                    // 需要读取的字节数（正常报文的完整长度-已经接收到的字节数）大于缓冲长度的话，只需要读取缓冲长度的字节
                    int receiveLength = Math.Min(bufferSize, (receiveLen - receiveByteCount));
                    try
                    {
                        // 参数：存放接收字节的数组，从第几个字节开始存入
                        if (socket.Poll(1000 * 1000, SelectMode.SelectRead))
                        {
                            var readLeng = socket.Receive(receiveBytes, receiveByteCount, receiveLength, SocketFlags.None);
                            if (readLeng == 0)
                            {
                                throw new Exception("未接收到响应数据");
                            }
                            receiveByteCount += readLeng;
                        }
                        else
                        {
                            throw new Exception("接收数据超时");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("接收响应数据异常!" + ex.Message);
                    }
                    flag++;
                }
                result.Datas = new List<byte>(receiveBytes);
            }
            catch (Exception ex)
            {
                result = new Result<byte>(false, ex.Message);
            }
            return result;
        }

        public void SendAndReceiveAsync(CommandTask serialTask, bool isRead = true)
        {
            throw new NotImplementedException();
        }
    }
}
