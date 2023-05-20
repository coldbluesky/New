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
    public class ModbusRtu : ModbusBase
    {
        protected override byte[] GetReadCommand(ModbusAddress modbusAddress, byte slaveId)
        {
            byte[] baseCommand = base.GetReadCommand(modbusAddress, slaveId);
            List<byte> bytes = new List<byte>(baseCommand);
            // RTU的处理方式
            bytes.AddRange(CRC16(bytes));
            // Ascii的处理方式
            // 计算LRC校验
            // 转换成对应的Ascii码编码
            // 添加报文头和尾

            return bytes.ToArray();
        }
        protected override byte[] GetWriteCommand(byte[] data, ModbusAddress address, byte slaveId)
        {
            byte[] baseCommand = base.GetWriteCommand(data, address, slaveId);
            List<byte> bytes = new List<byte>(baseCommand);
            // RTU的处理方式
            bytes.AddRange(CRC16(bytes));

            return bytes.ToArray();
        }


        public override Result Read(List<PointProperty> points, IComponent component, CommProperty commProperty)
        {
            try
            {
                // 1、开始对Points进行解析->Modbus地址  报文结构  ：功能码、起始地址、寄存器数量
                //List<ModbusAddress> modbus = new List<ModbusAddress>();
                //foreach (var p in points)
                //{
                //    ModbusAddress ma = this.AnalysisAddress(p);
                //    modbus.Add(ma);
                //}
                // 2、分批/分组
                // 应该知道需要进行多个少指令报文的生成 byte[]
                List<ModbusAddress> cmdMas = this.GroupAddress(points);

                // 准备好必要的信息后，进行byte[]组装   3个
                int pointsIndex = 0;
                foreach (var ma in cmdMas)
                {
                    List<byte> cmd = new List<byte>(this.GetReadCommand(ma, byte.Parse(commProperty.SlaveID)));
                    //cmd.AddRange(CRC16(cmd));
                    // 至此一帧请求报文结束

                    // 请求后需要返回的长度是多少？
                    // 根据请求的寄存器个数进行计算，一个寄存器返回2个字节   11-》22    3-》6   线圈：8个寄存器-》1byte
                    // ma.Length*2
                    // 01 03 16 00 00 00 00 00...... CRC(2)
                    // 异常情况下返回的长度是多少？
                    // 01 83 00 CRC(2)
                    // 向从站发起请求
                    int receiveLen = ma.Length * 2;
                    if (new int[] { 1, 2 }.Contains(ma.FuncCode))
                    {
                        receiveLen = (int)Math.Ceiling(ma.Length * 1.0 / 8);
                    }

                    // 发起请求 01 03 00 00 00 02 XX XX
                    Result<byte> read_result = component.SendAndReceive(cmd.ToArray(), receiveLen + 5, 5, this.Timeout).GetAwaiter().GetResult();
                    if (!read_result.IsSuccessed || read_result.Datas.Count == 0)
                        throw new Exception("响应报文接收异常！" + read_result.Message);

                    // read_reasult.Datas= 01 03 04 00 01 00 02 XX XX
                    // 做响应验证（CRC/异常返回报文）
                    // 校验与报文异常判断
                    bool code;
                    string msg;
                    // 检查CRC、异常码
                    if (!CheckException(read_result.Datas, out code, out msg))
                    {
                        throw new Exception(msg);
                    }
                    // 剥离出数据部分
                    read_result.Datas.RemoveRange(0, 3);

                    // 结果：byte[]{00 01 00 02}
                    // 放入 PointProperty  ValueBytes(byte[])  = byte[]{00 01};
                    // 放入 PointProperty  ValueBytes(byte[])  = byte[]{00 02};


                    // 使用byte[]进行返回
                    //                                           0          2   20-21      
                    // 第一个指令，byte[0-1]....byte[20-21]  ->  points[0] .... points[3]=10
                    // 第二个指令，byte[0-1]  byte[2-3]  byte[4-5]  -> points[4]
                    // 第三个指令，byte[0]....      ->   points[6]  points[7] ...  [特殊处理]

                    ModbusAddress startAddr = this.AnalysisAddress(points[pointsIndex]);
                    for (int i = 0; i < read_result.Datas.Count;)
                    {
                        //ma1.StartAddress
                        //points[pointsIndex].ByteCount

                        // 每次取2个字节   ->  起始地址/字节长度
                        ModbusAddress currentAddr = this.AnalysisAddress(points[pointsIndex]);
                        if (new int[] { 1, 2 }.Contains(ma.FuncCode))
                        {
                            // 处理线圈状态
                            //read_result.Datas[i] -> 8个寄存器
                            // 一个byte->n个point
                            // 另一种方式：把所有byte[]  统一转换成bit

                            string bitStr = Convert.ToString(read_result.Datas[i], 2).PadLeft(8, '0');   // 00000011
                            List<string> bitStrList = bitStr.ToArray().Select(b => b.ToString()).ToList();
                            //["0","0","0",.....,"1"]
                            bitStrList.Reverse();
                            //["1","1","0",.....,"0"]

                            // 循环  0 < 8
                            // 获取当前Point的起始地址
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
                            Array.Copy(read_result.Datas.ToArray(), i, points[pointsIndex].ValueBytes, 0, points[pointsIndex].ByteCount);

                            i += points[pointsIndex].ByteCount;

                            pointsIndex++;
                        }
                        // 最终转换成数据需要字节序的调整  -》  BitConverter

                        // 字节转数据（根协议无关）
                    }
                }

                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }

        // Action  - 处理最终的数据解析
        public override void ReadAsync(List<PointProperty> points, IComponent component, CommProperty commProperty, Action<Result> callback)
        {
            int completedCount = 0;
            Result result = new Result();
            List<byte[]> datas = new List<byte[]>();
            try
            {
                List<ModbusAddress> modbusAddresses = this.GroupAddress(points);
                //List<byte[]> cmds = this.GetReadCommand(GroupAddress(points), commProperty);
                //var requestCmds = this.GroupAddress(points);
                //foreach(var item in requestCmds)
                //{
                //    // 生成对应的ReadCommand
                //}
                int points_index = 0;
                foreach (var ma in modbusAddresses)
                {
                    byte[] cmd = this.GetReadCommand(ma, byte.Parse(commProperty.SlaveID));

                    byte[] lenBytes = new byte[] { cmd[5], cmd[4] };
                    int recLen = BitConverter.ToUInt16(lenBytes);
                    if (new byte[] { 0x01, 0x02 }.Contains(cmd[1]))
                        recLen = (int)Math.Ceiling(recLen * 1.0 / 8);
                    else
                        recLen = recLen * 2;


                    CommandTask serialTask = new CommandTask
                    {
                        Command = cmd,
                        ReceiveLen = recLen + 5,
                        ErrorLen = 5,
                        Timeout = this.Timeout,
                    };
                    serialTask.Callback = new Action<Result<byte>>(result =>
                    {
                        completedCount++;
                        try
                        {
                            if (!result.IsSuccessed || result.Datas.Count == 0) throw new Exception("响应报文接收异常！" + result.Message);

                            // 校验与报文异常判断
                            bool code;
                            string msg;
                            // 检查CRC、异常码
                            if (!CheckException(result.Datas, out code, out msg))
                            {
                                throw new Exception(msg);
                            }
                            // 解析数据部分
                            result.Datas.RemoveRange(0, 3);


                            // 拿到所有字节后做数据解析  [可进一步封装]
                            //foreach (var item in datas)
                            //{
                            ModbusAddress ma = this.AnalysisAddress(points[points_index]);
                            if (new int[] { 1, 2 }.Contains(ma.FuncCode))
                            {
                                // 每个字节对应8个线圈的量
                                List<byte> itemTemp = new List<byte>(result.Datas);
                                itemTemp.Reverse();
                                var valueTemp = string.Join("", itemTemp.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToList()).ToList();
                                valueTemp.Reverse();


                                int coilStart = ma.StartAddress;
                                while (ma.StartAddress - coilStart < valueTemp.Count)
                                {
                                    points[points_index].ValueBytes = new byte[] {
                                            (byte)int.Parse(valueTemp[ma.StartAddress - coilStart].ToString())
                                        };
                                    points_index++;
                                    if (points_index >= points.Count)
                                        break;
                                    ma = this.AnalysisAddress(points[points_index]);
                                }
                            }
                            else
                            {
                                int startAddr = ma.StartAddress;
                                for (int i = 0; i < result.Datas.Count;)
                                {
                                    ModbusAddress maTemp = this.AnalysisAddress(points[points_index]);
                                    int currentAddr = maTemp.StartAddress;
                                    int offsetIndex = currentAddr - startAddr;

                                    i = offsetIndex * 2;
                                    if (points[points_index].ValueType == typeof(bool))// 说明读取位数据，按Boolean类型处理
                                    {
                                        byte[] valueBytes = new byte[2];
                                        Array.Copy(result.Datas.ToArray(), i, valueBytes, 0, 2);

                                        List<byte> valueTemp = new List<byte>(valueBytes);
                                        var bitValues = string.Join("", valueTemp.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToList()).ToList();
                                        bitValues.Reverse();

                                        points[points_index].ValueBytes = new byte[] { (byte)int.Parse(bitValues[maTemp.BitAddress].ToString()) };

                                        if (points_index + 1 >= points.Count ||
                                            maTemp.StartAddress != this.AnalysisAddress(points[points_index + 1]).StartAddress)
                                            i += 2;
                                    }
                                    else // 按short,int,float...类型处理
                                    {
                                        Array.Copy(result.Datas.ToArray(), i, points[points_index].ValueBytes, 0, points[points_index].ByteCount);
                                        i += points[points_index].ByteCount;
                                    }
                                    points_index++;
                                }
                            }
                            //}

                            if (completedCount == modbusAddresses.Count)
                                callback?.Invoke(new Result());
                        }
                        catch (Exception ex)
                        {
                            callback?.Invoke(new Result(false, ex.Message));
                        }
                    });

                    component.SendAndReceiveAsync(serialTask);// 添加到执行队列
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(new Result(false, ex.Message));
            }
        }

        // 处理写入
        // 写入的情况下，可不可像读进行连续处理
        // 不太合适，    每个点位单写操作
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


                    Result<byte> send_result = component.SendAndReceive(write_cmd.ToArray(), 8, 5, this.Timeout).GetAwaiter().GetResult();
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
            int completedCount = 0;
            // 功能码：15/16
            // 写入线圈/保持寄存器（字/位）
            Result result = new Result();

            try
            {
                // 每一个点位一个写入（连续多写的情况，需要连续地址）
                foreach (var point in points)
                {
                    // 单个线圈写入
                    // 从站  功能码  写入地址  写入值  CRC
                    // 01    05      00 00     FF 00       // 原样回复
                    // 多寄存器写入（考虑多字节数字 float）
                    // 从站  功能码  写入地址  写入数量  字节数  写入值   CRC 
                    // 01    16      00 00     00 00     02      00 00
                    ModbusAddress ma = this.AnalysisAddress(point, false);
                    //commProperty.SlaveID
                    //ma.FuncCode
                    //ma.StartAddress
                    List<byte> write_cmd = new List<byte>(this.GetWriteCommand(point.ValueBytes, ma, byte.Parse(commProperty.SlaveID)));
                    //write_cmd.AddRange(CRC16(write_cmd));

                    //Result<byte> send_result = component.SendAndReceive(write_cmd, 8, 5, this.Timeout).GetAwaiter().GetResult();
                    //if (!send_result.IsSuccessed || send_result.Datas.Count == 0) throw new Exception("响应报文接收异常！" + send_result.Message);
                    CommandTask serialTask = new CommandTask()
                    {
                        Command = write_cmd.ToArray(),
                        ReceiveLen = 8,
                        ErrorLen = 5,
                        Timeout = this.Timeout
                    };
                    serialTask.Callback = new Action<Result<byte>>(result =>
                    {
                        completedCount++;
                        try
                        {
                            if (!result.IsSuccessed || result.Datas.Count == 0) throw new Exception("响应报文接收异常！" + result.Message);

                            // 校验与报文异常判断
                            bool code;
                            string msg;
                            // 检查CRC、异常码
                            if (!CheckException(result.Datas, out code, out msg))
                            {
                                throw new Exception(msg);
                            }

                            if (completedCount == points.Count)
                                callback?.Invoke(new Result());
                        }
                        catch (Exception ex)
                        {
                            callback?.Invoke(new Result(false, ex.Message));
                        }
                    });

                    // 添加的是写指令 
                    component.SendAndReceiveAsync(serialTask, false);
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(new Result(false, ex.Message));
            }
        }

        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private List<byte> CRC16(List<byte> value)
        {
            ushort poly = 0xA001;
            ushort crcInit = 0xFFFF;

            if (value == null || !value.Any())
                throw new ArgumentException("");

            //运算
            ushort crc = crcInit;
            for (int i = 0; i < value.Count; i++)
            {
                crc = (ushort)(crc ^ (value[i]));
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ poly) : (ushort)(crc >> 1);
                }
            }
            byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
            byte lo = (byte)(crc & 0x00FF);         //低位置

            List<byte> buffer = new List<byte>();
            buffer.Add(lo);
            buffer.Add(hi);
            return buffer;
        }

        private bool CheckException(List<byte> respBytes, out bool code, out string msg)
        {
            code = false;
            msg = "";

            // 响应报文的校验
            // LRC  -》 1个字节
            // TCP没有校验
            List<byte> checkCRC = respBytes.GetRange(respBytes.Count - 2, 2);
            respBytes.RemoveRange(respBytes.Count - 2, 2);
            var calcCRC = CRC16(respBytes);
            bool state = checkCRC.SequenceEqual(calcCRC);
            if (!state)
            {
                code = false; msg = "数据传输异常，校验不通过";
                return false;
            }


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
