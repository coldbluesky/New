using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Transfer
{
    internal class SerialUnit : TransferObject
    {
        private static readonly object trans_lock = new object();

        SerialPort serialPort;
        public SerialUnit()
        {
            serialPort = new SerialPort();
            this.TUnit = serialPort;
        }

        internal override Result Config(List<DevicePropItemEntity> props)
        {
            // 端口名称、波特率、数据位、校验位、停止位
            Result result = new Result();
            try
            {
                foreach (var item in props)
                {
                    object v = null;
                    PropertyInfo pi = serialPort.GetType().GetProperty(item.PropName.Trim(), BindingFlags.Public | BindingFlags.Instance);
                    if (pi == null) continue;

                    Type propType = pi.PropertyType;
                    if (propType.IsEnum)
                    {
                        v = Enum.Parse(propType, item.PropValue.Trim() as string);
                    }
                    else
                    {
                        v = Convert.ChangeType(item.PropValue.Trim(), propType);
                    }
                    pi.SetValue(serialPort, v);
                }

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }

            return result;
        }

        internal override Result Connect(int trycount = 30)
        {
            lock (trans_lock)
            {
                Result result = new Result();
                try
                {
                    int count = 0;
                    while (count < trycount)
                    {
                        if (serialPort.IsOpen)
                            break;

                        try
                        {
                            serialPort.Open();
                            break;
                        }
                        catch (System.IO.IOException)
                        {
                            Task.Delay(1).GetAwaiter().GetResult();
                            count++;
                        }
                    }
                    if (serialPort == null || !serialPort.IsOpen)
                        throw new Exception("串口打开失败");

                    ConnectState = true;
                }
                catch (Exception e)
                {
                    result.Status = false;
                    result.Message = e.Message;
                }
                return result;
            }
        }

        internal override Result<List<byte>> SendAndReceived(List<byte> req, int receiveLen, int errorLen, int timeout = 5000)
        {
            lock (trans_lock)
            {
                Result<List<byte>> result = new Result<List<byte>>();
                // 发送
                serialPort.Write(req.ToArray(), 0, req.Count);

                List<byte> respBytes = new List<byte>();
                try
                {
                    serialPort.ReadTimeout = timeout;
                    while (respBytes.Count < Math.Max(receiveLen, errorLen))
                    {
                        byte data = (byte)serialPort.ReadByte();
                        respBytes.Add(data);
                    }
                }
                catch (TimeoutException)
                {
                    if (respBytes.Count != errorLen && respBytes.Count != receiveLen)
                    {
                        result.Status = false;
                        result.Message = "接收报文超时";
                    }
                }
                catch (Exception e)
                {
                    result.Status = false;
                    result.Message = e.Message;
                }
                finally
                {
                    result.Data = respBytes;
                }
                return result;
            }
        }

        internal override Result Close()
        {
            if (serialPort != null)
                serialPort.Close();

            this.ConnectState = false;

            return new Result();
        }
    }
}
