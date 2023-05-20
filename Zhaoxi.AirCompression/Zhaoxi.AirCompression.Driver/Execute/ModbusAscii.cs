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
    public class ModbusAscii : ModbusBase
    {
        protected override byte[] GetReadCommand(ModbusAddress modbusAddress, byte slaveId)
        {
            byte[] bytes = base.GetReadCommand(modbusAddress, slaveId);

            List<byte> itemBytes = new List<byte>(bytes);
            itemBytes.Add(LRC(itemBytes));// 对基础报文进行LRC校验码计算

            // 需要进行Ascii码的转换
            // 转换成Ascii码字符16进制
            var hex = itemBytes.Select(b => b.ToString("X2"));
            // 转换成一个完整的字符串
            string hexStr = string.Join("", hex.ToArray());
            // 转换成AsciiList
            List<byte> asciiList = new List<byte>(Encoding.ASCII.GetBytes(hexStr));

            // 添加报头和报尾
            asciiList.Insert(0, 0x3A);
            asciiList.Add(13);
            asciiList.Add(10);

            return asciiList.ToArray();
        }
        protected override byte[] GetWriteCommand(byte[] data, ModbusAddress address, byte slaveId)
        {
            return base.GetWriteCommand(data, address, slaveId);
        }

        public override Result Read(List<PointProperty> points, IComponent component, CommProperty commProperty)
        {
            try
            {
                List<ModbusAddress> cmdMas = this.GroupAddress(points);

                // 准备好必要的信息后，进行byte[]组装   3个
                int pointsIndex = 0;
                foreach (var ma in cmdMas)
                {
                    List<byte> cmd = new List<byte>(this.GetReadCommand(ma, byte.Parse(commProperty.SlaveID)));

                    int recLen = ma.Length;
                    if (new int[] { 1, 2 }.Contains(ma.FuncCode))
                        recLen = (int)Math.Ceiling(recLen * 1.0 / 8) + 4;
                    else
                        recLen = recLen * 2 + 4;

                    Result<byte> read_result = component.SendAndReceive(cmd.ToArray(), recLen * 2 + 3, 11, this.Timeout).GetAwaiter().GetResult();
                    if (!read_result.IsSuccessed || read_result.Datas.Count == 0)
                        throw new Exception("响应报文接收异常！" + read_result.Message);

                    List<byte> respBytes = read_result.Datas;

                    // 解析 Ascii报文 -》 byte集合
                    // 去头掐尾
                    respBytes.RemoveAt(0);
                    respBytes.RemoveRange(respBytes.Count - 2, 2);

                    // 将Ascii码转换成ByteList
                    respBytes = AsciiArrayToByteArray(respBytes);



                    // 做响应验证（CRC/异常返回报文）
                    // 校验与报文异常判断
                    bool code;
                    string msg;
                    // 检查CRC、异常码
                    if (!CheckException(respBytes, out code, out msg))
                    {
                        throw new Exception(msg);
                    }
                    // 剥离出数据部分
                    respBytes.RemoveRange(0, 3);


                    // 通信取决于设备
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




        private byte LRC(List<byte> value)
        {
            if (value == null) return 0x00;

            int sum = 0;
            for (int i = 0; i < value.Count; i++)
            {
                sum += value[i];
            }

            sum = sum % 256;
            sum = 256 - sum;

            return (byte)sum;
        }

        private List<byte> AsciiArrayToByteArray(List<byte> value)
        {
            List<string> asciiStrList = new List<string>();
            foreach (var item in value)
            {
                asciiStrList.Add(((char)item).ToString());
            }

            // 将每两个Ascii字符组成一个16进制 转换成字节  ---  对应：3
            List<byte> resultBytes = new List<byte>();
            for (int i = 0; i < asciiStrList.Count; i++)
            {
                var stringHex = asciiStrList[i].ToString() + asciiStrList[++i].ToString();
                resultBytes.Add(Convert.ToByte(stringHex, 16));
            }
            return resultBytes;
        }

        private bool CheckException(List<byte> respBytes, out bool code, out string msg)
        {
            code = false;
            msg = "";

            // 响应报文的校验
            // LRC  -》 1个字节
            // TCP没有校验
            byte checkLRC = respBytes[respBytes.Count - 1];
            respBytes.RemoveRange(respBytes.Count - 1, 1);
            var calcLRC = LRC(respBytes);
            bool state = checkLRC.Equals(calcLRC);
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
