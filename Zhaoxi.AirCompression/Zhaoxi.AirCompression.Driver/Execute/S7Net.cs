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
    public class S7Net : ExecuteBase
    {
        private int _pduSize = 240;

        Dictionary<byte, string> HeaderErrors = new Dictionary<byte, string>()
        {
            {0x00,"无错误" },
            {0x81,"应用程序关系错误" },
            {0x82,"对象定义错误" },
            {0x83,"无资源可用错误" },
            {0x84,"服务处理错误" },
            {0x85,"请求错误" },
            {0x87,"访问错误" }
        };

        Dictionary<byte, string> DataItemReturnCodes = new Dictionary<byte, string>()
        {
            { 0xff,"请求成功"},
            { 0x01,"硬件错误"},
            { 0x03,"对象不允许访问"},
            { 0x05,"地址越界，所需的地址超出此PLC的极限"},
            { 0x06,"请求的数据类型与存储类型不一致"},
            { 0x07,"日期类型不一致"},
            { 0x0a,"对象不存在"}
        };

        Dictionary<string, SiemensAreaTypes> AreaTypeDic = new Dictionary<string, SiemensAreaTypes>()
        {
            {"I",SiemensAreaTypes.INPUT },
            {"Q",SiemensAreaTypes.OUTPUT },
            {"M",SiemensAreaTypes.MERKER },
            {"V",SiemensAreaTypes.DATABLOCK }
        };

        // 地址解析
        private SiemensAddress AnalysisAddress(PointProperty point)
        {
            SiemensAddress siemensAddress = new SiemensAddress();
            siemensAddress.DBNumber = 0;
            // 前面两个字符
            string str = point.Address.Substring(0, 2).ToUpper();
            if (str.ToUpper() == "DB")
            {
                string[] strArrays = point.Address.Split('.');// { DB1     .      100}

                // 区域类型  DB
                siemensAddress.AreaType = SiemensAreaTypes.DATABLOCK;
                siemensAddress.DBNumber = int.Parse(strArrays[0].Substring(2));

                siemensAddress.ByteAddress = int.Parse(strArrays[1]);
                siemensAddress.ByteCount = point.ByteCount;  // 暂不考虑String类型

                if (strArrays.Length == 3 && int.TryParse(strArrays[2], out int bitValue))
                {
                    if (bitValue > 7)
                        throw new Exception("Bit地址设置错误，只允许在0-7范围内");
                    siemensAddress.BitAddress = (byte)bitValue;

                    siemensAddress.ByteCount = 1;
                }
            }
            else if (new string[] {
                "I", "Q", "M", "V"
            }.Contains(point.Address[0].ToString().ToUpper()))
            {
                if (str[0].ToString() == "V")
                    siemensAddress.DBNumber = 1;

                if (AreaTypeDic.ContainsKey(str[0].ToString()))
                {
                    siemensAddress.AreaType = AreaTypeDic[str[0].ToString()];
                }
                siemensAddress.ByteCount = point.ByteCount;// 暂不考虑String类型


                string[] address = point.Address.Split('.');
                siemensAddress.ByteAddress = int.Parse(address[0].Substring(1));
                if (address.Length == 2 && int.TryParse(address[1], out int bitValue))
                {
                    if (bitValue > 7)
                        throw new Exception("Bit地址设置错误，只允许在0-7范围内");
                    siemensAddress.BitAddress = (byte)bitValue;

                    siemensAddress.ByteCount = 1;
                }
            }
            if (point.ValueType == typeof(bool))
                siemensAddress.ByteCount = 1;

            return siemensAddress;
        }

        // 地址分组  -》  Param-Item
        public List<SiemensAddress> GroupAddress(List<PointProperty> points)
        {
            List<SiemensAddress> address = new List<SiemensAddress>();
            SiemensAddress sa = new SiemensAddress();
            SiemensAddress saLast = null;
            foreach (var point in points)
            {
                // 1、地址转换
                SiemensAddress saCurrent = this.AnalysisAddress(point);

                // 判断是不必于当前组
                // 功能码要一样、
                if (saCurrent.AreaType != sa.AreaType)// 暂时不考虑顺序问题
                {
                    saLast = saCurrent;
                    sa = new SiemensAddress(saCurrent); // 当前组的第一个
                    address.Add(sa);
                }
                else
                {
                    // 判断 地址相对当前组的第一个地址有没有超越20个寄存器
                    // 20是个参考值
                    // 
                    if (saCurrent.ByteAddress - sa.ByteAddress + saCurrent.ByteCount - saLast.ByteCount > 32)
                    {
                        saLast = saCurrent;
                        sa = new SiemensAddress(saCurrent); // 当前组的第一个
                        address.Add(sa);
                    }
                    else
                    {
                        // 没有超越的情况下，把地址长度增加
                        // item:当前这个地址
                        // lastMa:上一次的地址信息
                        sa.ByteCount += (saCurrent.ByteAddress - saLast.ByteAddress + saCurrent.ByteCount - saLast.ByteCount);
                    }
                }
                saLast = saCurrent;
            }

            return address;
        }

        private List<byte[]> GetReadCommands(List<SiemensAddress> address)
        {
            // List<SiemensAddress>每个内容都是一个Item
            List<byte[]> cmds = new List<byte[]>();

            List<byte> paramBytes = new List<byte>();
            int byteCount = 0;
            int itemCount = 0;
            for (int i = 0; i < address.Count; i++)
            {
                var item = address[i];
                if (item.ByteCount + 23 + byteCount < this._pduSize)
                {
                    itemCount++;
                    byteCount += item.ByteCount + 4;

                    List<byte> paramTemp = GetParamItemBytes(item);

                    paramBytes.AddRange(paramTemp);
                }
                else
                {
                    paramBytes.Insert(0, (byte)itemCount);
                    paramBytes.Insert(0, 0x04);
                    cmds.Add(this.GetRequestCommands(paramBytes).ToArray());

                    paramBytes.Clear();
                    itemCount = 0;
                    byteCount = 0;
                    i--;
                }
            }
            if (paramBytes.Count > 0)
            {
                paramBytes.Insert(0, (byte)itemCount);
                paramBytes.Insert(0, 0x04);
                cmds.Add(this.GetRequestCommands(paramBytes).ToArray());

            }

            return cmds;
        }

        private List<byte> GetParamItemBytes(SiemensAddress sa)
        {
            List<byte> paramBytes = new List<byte>();
            // 组装Parameter Item
            paramBytes.Add(0x12);
            paramBytes.Add(0x0a);
            paramBytes.Add(0x10);
            paramBytes.Add(0x02);// 读取类型  Bit  02Byte  03Char  04Word
                                 // 写入长度
            paramBytes.Add((byte)(sa.ByteCount / 256 % 256));
            paramBytes.Add((byte)(sa.ByteCount % 256));
            // DB块编号   200Smart V区  DB1
            paramBytes.Add((byte)(sa.DBNumber / 256 % 256));
            paramBytes.Add((byte)(sa.DBNumber % 256));
            // 数据区域
            paramBytes.Add((byte)sa.AreaType);  //81 I   82  Q   83  M   84DB
                                                // 地址
                                                // Byte:100   Bit:0

            //int address = startAddr * 8 + bitAddr;
            int addr = (sa.ByteAddress << 3);
            paramBytes.Add((byte)(addr / 256 / 256 % 256));
            paramBytes.Add((byte)(addr / 256 % 256));
            paramBytes.Add((byte)(addr % 256));

            return paramBytes;
        }

        private List<byte> GetRequestCommands(List<byte> paramBytes, List<byte> dataBytes = null)
        {
            // TPKT
            List<byte> tpktBytes = new List<byte>();
            tpktBytes.Add(0x03);
            tpktBytes.Add(0x00);
            // --------整个字节数组的长度

            // COTP
            List<byte> cotpBytes = new List<byte>();
            cotpBytes.Add(0x02);
            cotpBytes.Add(0xf0);
            cotpBytes.Add(0x80);

            // Header
            List<byte> headerBytes = new List<byte>();
            headerBytes.Add(0x32);
            headerBytes.Add(0x01);
            headerBytes.Add(0x00);
            headerBytes.Add(0x00);
            headerBytes.Add(0x00);
            headerBytes.Add(0x00);
            // 添加Parameter字节数组的长度
            // 添加Data字节数组的长度


            // 开始组装paramBytes
            // 开始组装 dataBytes
            if (paramBytes != null)
            {
                // 拼装Header&Parameter
                headerBytes.Add((byte)(paramBytes.Count / 256 % 256));
                headerBytes.Add((byte)(paramBytes.Count % 256));
            }
            if (dataBytes != null)
            {
                headerBytes.Add((byte)(dataBytes.Count / 256 % 256));
                headerBytes.Add((byte)(dataBytes.Count % 256));
            }
            else
            {
                headerBytes.Add(0x00);
                headerBytes.Add(0x00);
            }

            if (paramBytes != null)
                headerBytes.AddRange(paramBytes);
            if (dataBytes != null)
                headerBytes.AddRange(dataBytes);
            // 拼装COTP&Header
            cotpBytes.AddRange(headerBytes);
            //拼装 TPKT&COTP
            // tpkt现有长度+报文总长度2个字节+COTP长度
            int count = tpktBytes.Count + 2 + cotpBytes.Count;
            tpktBytes.Add((byte)(count / 256 % 256));
            tpktBytes.Add((byte)(count % 256));
            tpktBytes.AddRange(cotpBytes);

            // 检查请求报文长度是否超过PDU Size
            if (tpktBytes.Count > _pduSize)
                throw new Exception("请求报文长度超过PLC的PDU最大值");


            return tpktBytes;
        }

        // COTP的通信
        // SetupCommunication的通信
        public override Result BuildCommunication(IComponent component, CommProperty commProperty)
        {
            // COTP
            Result cotp_result = COTPConnection(component, commProperty.Rack, commProperty.Slot);
            if (!cotp_result.IsSuccessed) return cotp_result;
            // Setup Communication
            Result setup_result = SetupCommunication(component);
            if (!setup_result.IsSuccessed) return setup_result;

            return new Result();
        }

        // COTP
        private Result COTPConnection(IComponent component, int rack, int slot)
        {
            Result result = new Result();

            byte[] cotpBytes = new byte[] {
                // TPKT
                0x03,0x00,0x00,0x16,
                // COTP
                0x11,0xe0,
                0x00,0x00,0x00,0x00,0x00,
                    
                // Parameter-code  tpdu-size
                0xc0,0x01,0x0a,
                // Parameter-code  src-tsap
                0xc1,0x02,0x10,0x00,
                // Parameter-code  dst-tsap
                0xc2,0x02,0x03,(byte)(rack*32+slot),
            };

            try
            {
                Result<byte> resp = component.SendAndReceive(cotpBytes, 22, 22, this.Timeout).GetAwaiter().GetResult();
                //解析响应报文
                //byte[] respBytes = new byte[22];
                //int count = socket.Receive(respBytes, 0, 22, SocketFlags.None);
                if (resp.Datas[5] != 0xd0)
                {
                    throw new Exception("COTP连接响应异常");
                }
            }
            catch (Exception ex)
            {
                result = new Result(false, "COTP连接未建立！" + ex.Message);
            }

            return result;
        }
        // Setup
        private Result SetupCommunication(IComponent component)
        {
            Result result = new Result();
            byte[] setupBytes = new byte[] {
                // TPKT
                0x03,0x00,0x00,0x19,
                    // COTP
                0x02,0xf0,0x80,
                // Header
                0x32,0x01,
                0x00,0x00,0x00,0x00,
                // PL
                0x00,0x08,
                // DL
                0x00,0x00,
                // Parameter
                0xf0,
                0x00,
                0x00,0x03,0x00,0x03,0x03,0xc0
            };
            try
            {
                Result<byte> setp_result = component.SendAndReceive(setupBytes, 27, 27, this.Timeout).GetAwaiter().GetResult();
                //socket.Send(setupBytes);
                //byte[] respBytes = new byte[27];
                //int count = socket.Receive(respBytes);
                // 拿到PDU长度   后续进行报文组装和接收的时候可以参考
                byte[] pdu_size = new byte[2];
                pdu_size[0] = setp_result.Datas[26];
                pdu_size[1] = setp_result.Datas[25];

                this._pduSize = BitConverter.ToInt16(pdu_size);
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = "Setup通信未建立！" + ex.Message;
            }
            return result;
        }

        public override Result Read(List<PointProperty> points, IComponent component, CommProperty commProperty)
        {
            try
            {
                // 拿到分组后的地址集合  n个Item
                List<SiemensAddress> addrList = this.GroupAddress(points);
                // 根据分组地址进行报文的组装
                // 几个Item放在一个请求里
                // 需要把所有的Item进行分组
                List<byte[]> cmds = GetReadCommands(addrList);

                int point_index = 0;
                foreach (var cmd in cmds)
                {
                    // 每一个cmd属于一个请求
                    // 计算每次计算的字节
                    // 先取4个字节-再取剩余字节
                    var send_result = component.SendAndReceive(cmd, 4, 4, this.Timeout).GetAwaiter().GetResult();
                    if (!send_result.IsSuccessed) throw new Exception(send_result.Message);

                    // 解析所有字节数，并获取剩余字节
                    byte[] lenBytes = new byte[2];
                    lenBytes[0] = send_result.Datas[3];
                    lenBytes[1] = send_result.Datas[2];
                    short len = BitConverter.ToInt16(lenBytes);
                    len -= 4;

                    send_result = component.SendAndReceive(null, len, len, this.Timeout).GetAwaiter().GetResult();
                    if (!send_result.IsSuccessed) throw new Exception(send_result.Message);

                    List<byte> buffer = send_result.Datas;
                    // 判断是否有异常
                    if (buffer[13] != 0x00)
                        throw new Exception($"{buffer[13]}:{HeaderErrors[buffer[13]]}");
                    else
                    {
                        for (int index = 17; index < buffer.Count;)
                        {
                            if (buffer[index] == 0xff)
                            {
                                // 需要取数据部分的字节
                                /// 解析字节数组
                                lenBytes[0] = buffer[index + 3];
                                lenBytes[1] = buffer[index + 2];
                                ushort dataLen = (ushort)(BitConverter.ToUInt16(lenBytes) / 8);
                                // 判断每个ITEM的字节数

                                index += 4;
                                byte[] dataBytes = new byte[dataLen];// 某一个Item的数据部分
                                Array.Copy(buffer.ToArray(), index, dataBytes, 0, dataLen);
                                index += dataLen + (dataLen % 2);// 移动数组指针


                                // 分配字节
                                int startAddr = this.AnalysisAddress(points[point_index]).ByteAddress;
                                for (int i = 0; i < dataBytes.Length;)
                                {
                                    var currentAddr = this.AnalysisAddress(points[point_index]);
                                    i = currentAddr.ByteAddress - startAddr;
                                    if (points[point_index].ValueType == typeof(bool))
                                    {
                                        byte boolByte = dataBytes[i];
                                        List<char> boolChars = new List<char>(Convert.ToString(boolByte, 2).PadLeft(8, '0').ToArray());
                                        boolChars.Reverse();
                                        points[point_index].ValueBytes = new byte[] {
                                                    (byte)((boolChars[currentAddr.BitAddress]=='1')?0x01:0x00)
                                                };

                                        if (point_index + 1 >= points.Count ||
                            currentAddr.ByteAddress != this.AnalysisAddress(points[point_index + 1]).ByteAddress ||
                            currentAddr.AreaType != this.AnalysisAddress(points[point_index + 1]).AreaType)
                                            i += 1;
                                    }
                                    else
                                    {
                                        Array.Copy(
                                            dataBytes,
                                            i,
                                            points[point_index].ValueBytes,
                                            0,
                                            points[point_index].ByteCount);
                                        i += points[point_index].ByteCount;
                                    }
                                    point_index++;
                                }
                            }
                            else
                                throw new Exception(DataItemReturnCodes[buffer[index]]);
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

        public override Result Write(List<PointProperty> points, IComponent component, CommProperty commProperty)
        {
            return base.Write(points, component, commProperty);
        }
    }
}
