using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Driver.Base;

namespace Zhaoxi.AirCompression.Driver.Component
{
    public class ComponentSerial : IComponent
    {
        private SerialPort _serialPort = null;

        private Stopwatch stopwatch = new Stopwatch();

        private CommProperty _commProperty;

        // 单列表有问题！
        // 写入
        //private List<SerialTask> serialTask = new List<SerialTask>();
        // 双任务列表
        private List<CommandTask> serialReadTasks = new List<CommandTask>();
        private List<CommandTask> serialWriteTasks = new List<CommandTask>();
        private Task task = null;
        public ComponentSerial(CommProperty commProperty)
        {
            _commProperty = commProperty;
            //CreateSerial();

            this.ExecuteTasks();
        }

        private CancellationTokenSource cancelToken = new CancellationTokenSource();
        private void ExecuteTasks()
        {
            task = Task.Run(new Action(async () =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    if (serialWriteTasks.Count > 0)
                    {
                        Debug.WriteLine("----写请求：" + string.Join(" ", serialWriteTasks[0].Command.Select(c => c.ToString("X2")).ToList()));
                        // 执行
                        var result = await this.SendAndReceive(serialWriteTasks[0].Command, serialWriteTasks[0].ReceiveLen, serialWriteTasks[0].ErrorLen, serialWriteTasks[0].Timeout);

                        serialWriteTasks[0].Callback?.Invoke(result);

                        serialWriteTasks.RemoveAt(0);
                    }
                    else if (serialReadTasks.Count > 0)
                    {
                        if (serialReadTasks == null)
                            Debug.WriteLine("1");
                        if (serialReadTasks[0] == null)
                            Debug.WriteLine("2");
                        if (serialReadTasks[0].Command == null)
                            Debug.WriteLine("3");
                        if (serialReadTasks[0].Command.Select(c => c.ToString("X2")) == null)
                            Debug.WriteLine("4");
                        Debug.WriteLine("=====读请求：" + string.Join(" ", serialReadTasks[0].Command.Select(c => c.ToString("X2")).ToList()));
                        // 执行
                        var result = await this.SendAndReceive(serialReadTasks[0].Command, serialReadTasks[0].ReceiveLen, serialReadTasks[0].ErrorLen, serialReadTasks[0].Timeout);

                        serialReadTasks[0].Callback?.Invoke(result);

                        serialReadTasks.RemoveAt(0);
                    }
                }
            }), cancelToken.Token);
        }

        private void CreateSerial()
        {
            if (_serialPort == null) _serialPort = new SerialPort();
            // 参数
            _serialPort.PortName = _commProperty.PortName;
            _serialPort.BaudRate = int.Parse(_commProperty.BaudRate);
            _serialPort.DataBits = int.Parse(_commProperty.DataBits);
            _serialPort.StopBits = _commProperty.StopBits;
            _serialPort.Parity = _commProperty.Parity;
            _serialPort.ReadTimeout = 10;

            //_serialPort.ReceivedBytesThreshold = 1000;
            //_serialPort.ReadBufferSize=
        }

        public async Task<Result> Connect(int timeout = 2000)
        {
            Result result = new Result();
            try
            {
                if (_serialPort == null)
                    CreateSerial();

                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout)
                {
                    if (_serialPort.IsOpen)
                        return result;

                    try
                    {
                        _serialPort.Open();
                        return result;
                    }
                    catch (System.IO.IOException)
                    {
                        await Task.Delay(1);
                        continue;
                    }
                }
                if (_serialPort == null || !_serialPort.IsOpen)
                    throw new Exception("串口打开失败");
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
            }
            finally
            {
                stopwatch.Reset();
            }

            return result;
        }

        public Result Close()
        {
            Result result = new Result();
            if (serialReadTasks.Count > 0 || serialWriteTasks.Count > 0) return new Result(false, "");
            try
            {
                cancelToken.Cancel();
                task.ConfigureAwait(false);

                if (_serialPort != null && _serialPort.IsOpen)
                    _serialPort.Close();
                _serialPort = null;

                return result;
            }
            catch (Exception ex)
            {
                result = new Result(false, ex.Message);
            }

            return result;
        }

        public async Task<Result<byte>> SendAndReceive(byte[] command, int receiveLen, int errorByteLen, int timeout)
        {
            Result<byte> result = new Result<byte>();

            try
            {
                stopwatch.Restart();
                _serialPort.Write(command, 0, command.Length);

                List<byte> receiveBytes = new List<byte>();

                while (stopwatch.ElapsedMilliseconds < timeout && receiveBytes.Count < receiveLen)
                {
                    //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
                    try
                    {
                        byte data = (byte)_serialPort.ReadByte();
                        receiveBytes.Add(data);
                        await Task.Delay(1);
                    }
                    catch
                    {
                        if (receiveBytes.Count == errorByteLen) break;
                    }
                }
                if (receiveBytes.Count == 0)
                {
                    //
                }
                stopwatch.Reset();

                result.Datas = receiveBytes;
            }
            catch (Exception ex)
            {
                result = new Result<byte>(false, ex.Message);
            }

            return result;
        }

        public void SendAndReceiveAsync(CommandTask command, bool isRead = true)
        {
            if (isRead)
                serialReadTasks.Add(command);
            else
                serialWriteTasks.Add(command);
        }
    }
}
