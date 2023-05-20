using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Driver.Base;
using Zhaoxi.AirCompression.Driver.Base.Address;
using Zhaoxi.AirCompression.Driver.Component;

namespace Zhaoxi.AirCompression.Driver.Execute
{
    public class ModbusTcp : ModbusBase
    {
        // 面向对象的基本特性
        protected override byte[] GetReadCommand(ModbusAddress modbusAddress, byte slaveId)
        {
            byte[] bytes = base.GetReadCommand(modbusAddress, slaveId);

            // 添加MBAP
            List<byte> byteList = new List<byte>();
            byteList.Add(0x00);// TransactionID 
            byteList.Add(0x00);// TransactionID   
            byteList.Add(0x00);
            byteList.Add(0x00); // Modbus标记
            byteList.Add((byte)(bytes.Length / 256));
            byteList.Add((byte)(bytes.Length % 256)); // 后续字节数
            byteList.AddRange(bytes);

            return byteList.ToArray();
        }

        protected override byte[] GetWriteCommand(byte[] data, ModbusAddress address, byte slaveId)
        {
            byte[] bytes = base.GetWriteCommand(data, address, slaveId);

            // 添加MBAP
            List<byte> byteList = new List<byte>();
            byteList.Add(0x00);// TransactionID 
            byteList.Add(0x00);// TransactionID   
            byteList.Add(0x00);
            byteList.Add(0x00); // Modbus标记
            byteList.Add((byte)(bytes.Length / 256));
            byteList.Add((byte)(bytes.Length % 256)); // 后续字节数
            byteList.AddRange(bytes);

            return byteList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="component">通信组件  Socket</param>
        /// <param name="commProperty"></param>
        /// <returns></returns>
        public override Result Read(List<PointProperty> points, IComponent component, CommProperty commProperty)
        {
            try
            {
                // 地址分组
                List<ModbusAddress> cmdMas = this.GroupAddress(points);

                // 准备好必要的信息后，进行byte[]组装   3个
                int pointsIndex = 0;
                foreach (var ma in cmdMas)
                {
                    List<byte> cmd = new List<byte>(this.GetReadCommand(ma, byte.Parse(commProperty.SlaveID)));

                    int recLen = 0;
                    if (new int[] { 1, 2 }.Contains(ma.FuncCode))
                        recLen = (int)Math.Ceiling(ma.Length * 1.0 / 8);
                    else
                        recLen = ma.Length * 2;

                    /// Modbus可以进行字节的计算
                    /// S7 2-3个字节 表示的是所有字节长度
                    /// FinsTCP/QnA-3E    
                    Result<byte> read_result = component.SendAndReceive(
                        cmd.ToArray(),  // 报文byte[]
                        recLen + 9,     //将接收的字节数
                        9,              // 异常情况下的字节数
                        this.Timeout    // 超时时间 
                        ).GetAwaiter().GetResult();
                    if (!read_result.IsSuccessed || read_result.Datas.Count == 0)
                        throw new Exception("响应报文接收异常！" + read_result.Message);


                    List<byte> respBytes = read_result.Datas;
                    respBytes.RemoveRange(0, 6);


                    // 做响应验证（CRC/异常返回报文）
                    // 校验与报文异常判断
                    bool code;
                    string msg;
                    // 检查CRC、异常码
                    // 00 00 00 00 00 06       01 03 02 00 01
                    if (!CheckException(respBytes, out code, out msg))
                    {
                        throw new Exception(msg);
                    }
                    // 剥离出数据部分
                    respBytes.RemoveRange(0, 3);


                    // AnalysiceData(byte[],Points,Index);
                    ModbusAddress startAddr = this.AnalysisAddress(points[pointsIndex]);
                    for (int i = 0; i < respBytes.Count;)
                    {

                        ModbusAddress currentAddr = this.AnalysisAddress(points[pointsIndex]);
                        if (new int[] { 1, 2 }.Contains(ma.FuncCode))
                        {


                            string bitStr = Convert.ToString(respBytes[i], 2).PadLeft(8, '0');   // 00000011
                            List<string> bitStrList = bitStr.ToArray().Select(b => b.ToString()).ToList();
                            //["0","0","0",.....,"1"]
                            bitStrList.Reverse();

                            int coilStart = currentAddr.StartAddress;
                            while (currentAddr.StartAddress - coilStart < bitStrList.Count)
                            {
                                points[pointsIndex].ValueBytes = new byte[] {
                                    (byte)int.Parse(bitStrList[currentAddr.StartAddress - coilStart].ToString())    // 0x01
                                };
                                pointsIndex++;
                                if (pointsIndex >= points.Count)
                                    break;
                                currentAddr = this.AnalysisAddress(points[pointsIndex]);
                            }
                            i++;
                        }
                        else
                        {
                            int offsetIndex = currentAddr.StartAddress - startAddr.StartAddress;
                            i = offsetIndex * 2;

                            // 对应的数据字节复制出来，放入点位对象中
                            Array.Copy(respBytes.ToArray(), i, points[pointsIndex].ValueBytes, 0, points[pointsIndex].ByteCount);

                            i += points[pointsIndex].ByteCount;

                            pointsIndex++;
                        }

                    }
                }

                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }

        public override void ReadAsync(List<PointProperty> points, IComponent component, CommProperty commProperty, Action<Result> callback)
        {
            callback.Invoke(this.Read(points, component, commProperty));
        }

        public override Result Write(List<PointProperty> points, IComponent component, CommProperty commProperty)
        {
            // points[0].ValueBytes
            // 不考虑同时读写的操作
            try
            {
                foreach (var point in points)
                {
                    ModbusAddress ma = this.AnalysisAddress(point, false);
                    // 
                    List<byte> write_cmd = new List<byte>(this.GetWriteCommand(point.ValueBytes, ma, byte.Parse(commProperty.SlaveID)));


                    Result<byte> send_result = component.SendAndReceive(write_cmd.ToArray(), 12, 9, this.Timeout).GetAwaiter().GetResult();
                    if (!send_result.IsSuccessed || send_result.Datas.Count == 0) throw new Exception("响应报文接收异常！" + send_result.Message);

                    // 校验与报文异常判断
                    bool code;
                    string msg;
                    // 检查CRC、异常码
                    if (!CheckException(send_result.Datas, out code, out msg))
                    {
                        throw new Exception(msg);
                    }
                }
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }

        public override void WriteAsync(List<PointProperty> points, IComponent component, CommProperty commProperty, Action<Result> callback)
        {
            callback?.Invoke(Write(points, component, commProperty));
        }

        // 元组
        private bool CheckException(List<byte> respBytes, out bool code, out string msg)
        {
            code = false;
            msg = "";

            // 功能码
            int func = respBytes[1];
            if ((func & 0x80) == 0x80)
            {
                byte stateCode = respBytes[2];
                // 如果异常，再检查异常Code
                code = false; msg = Errors.ContainsKey(stateCode) ? Errors[stateCode] : "自定义异常";
                return false;
            }

            return true;
        }
    }
}
