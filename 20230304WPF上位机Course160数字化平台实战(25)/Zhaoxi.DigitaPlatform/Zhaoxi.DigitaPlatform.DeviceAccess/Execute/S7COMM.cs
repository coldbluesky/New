using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Transfer;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Execute
{
    internal class S7COMM : ExecuteObject
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
        private SiemensAddress AnalysisAddress(VariableProperty point)
        {
            SiemensAddress siemensAddress = new SiemensAddress();
            siemensAddress.DBNumber = 0;
            // 前面两个字符
            string str = point.VarAddr.Substring(0, 2).ToUpper();
            if (str.ToUpper() == "DB")
            {
                string[] strArrays = point.VarAddr.Split('.');// { DB1     .      100}

                // 区域类型  DB
                siemensAddress.AreaType = SiemensAreaTypes.DATABLOCK;
                siemensAddress.DBNumber = int.Parse(strArrays[0].Substring(2));

                siemensAddress.ByteAddress = int.Parse(strArrays[1]);
                siemensAddress.ByteCount = Marshal.SizeOf(point.ValueType);

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
            }.Contains(point.VarAddr[0].ToString().ToUpper()))
            {
                if (str[0].ToString() == "V")
                    siemensAddress.DBNumber = 1;

                if (AreaTypeDic.ContainsKey(str[0].ToString()))
                {
                    siemensAddress.AreaType = AreaTypeDic[str[0].ToString()];
                }
                siemensAddress.ByteCount = Marshal.SizeOf(point.ValueType);


                string[] address = point.VarAddr.Split('.');
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

        public override Result<List<CommAddress>> GroupAddress(List<VariableProperty> variables)
        {
            Result<List<CommAddress>> result = new Result<List<CommAddress>>();

            SiemensAddress sa = new SiemensAddress();
            SiemensAddress saLast = null;
            foreach (var point in variables)
            {
                // 1、地址转换
                SiemensAddress saCurrent = this.AnalysisAddress(point);

                // 判断是不必于当前组
                // 功能码要一样、
                if (saCurrent.AreaType != sa.AreaType)// 暂时不考虑顺序问题
                {
                    saLast = saCurrent;
                    sa = new SiemensAddress(saCurrent); // 当前组的第一个
                    result.Data.Add(sa);
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
                        result.Data.Add(sa);
                    }
                    else
                    {
                        // 没有超越的情况下，把地址长度增加
                        // item:当前这个地址
                        // lastMa:上一次的地址信息
                        sa.ByteCount += saCurrent.ByteAddress - saLast.ByteAddress + saCurrent.ByteCount - saLast.ByteCount;
                    }
                }
                sa.Variables.Add(saCurrent);

                saLast = saCurrent;
            }

            return result;
        }

        internal override Result Match(List<DevicePropItemEntity> props, List<TransferObject> tos)
        {
             var ps = props.Where(p => p.PropName == "IP" || p.PropName == "Port").Select(p => p.PropValue).ToList();
            return this.Match(props, tos, ps, "SocketTcpUnit");
        }

        public override Result Read(List<CommAddress> addresses)
        {
            try
            {
                var prop = this.Props.FirstOrDefault(p => p.PropName == "Timeout");
                int timeout = 5000;
                if (prop != null)
                    int.TryParse(prop.PropValue, out timeout);

                int point_index = 0;
                int read_index = 0;
                foreach (var addr in addresses)
                {
                    if (!this.TransferObject.ConnectState)
                    {
                        Result connet_result = this.Connect();
                        if (!connet_result.Status) return connet_result;
                    }

                    List<byte> cmd = this.GetReadCommands(addresses, ref read_index);
                    // 每一个cmd属于一个请求
                    // 计算每次计算的字节
                    var send_result = this.TransferObject.SendAndReceived(new List<byte>(cmd), 4, timeout, this.CalcDataLength);
                    if (!send_result.Status) throw new Exception(send_result.Message);

                    // 解析所有字节数，并获取剩余字节
                    byte[] lenBytes = new byte[2];
                    //lenBytes[0] = send_result.Data[3];
                    //lenBytes[1] = send_result.Data[2];
                    short len = 0; BitConverter.ToInt16(lenBytes);
                    //len -= 4;

                    //send_result = TransferObject.SendAndReceived(null, timeout);
                    //if (!send_result.Status) throw new Exception(send_result.Message);

                    List<byte> buffer = send_result.Data;
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
                                int startAddr = (addresses[point_index] as SiemensAddress).ByteAddress;
                                for (int i = 0; i < dataBytes.Length;)
                                {
                                    var currentAddr = addresses[point_index] as SiemensAddress;
                                    i = currentAddr.ByteAddress - startAddr;
                                    if (currentAddr.ValueType == typeof(bool))
                                    {
                                        byte boolByte = dataBytes[i];
                                        List<char> boolChars = new List<char>(Convert.ToString(boolByte, 2).PadLeft(8, '0').ToArray());
                                        boolChars.Reverse();
                                        currentAddr.ValueBytes = new byte[] {
                                                    (byte)((boolChars[currentAddr.BitAddress]=='1')?0x01:0x00)
                                                };

                                        if (point_index + 1 >= addresses.Count ||
                            currentAddr.ByteAddress != (addresses[point_index + 1] as SiemensAddress).ByteAddress ||
                            currentAddr.AreaType != (addresses[point_index + 1] as SiemensAddress).AreaType)
                                            i += 1;
                                    }
                                    else
                                    {
                                        Array.Copy(
                                            dataBytes,
                                            i,
                                            currentAddr.ValueBytes,
                                            0,
                                            currentAddr.ByteCount);
                                        i += currentAddr.ByteCount;
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

        private int CalcDataLength(byte[] data)
        {
            int length = 0;
            if (data != null && data.Length > 0)
            {
                length = BitConverter.ToUInt16(new byte[] { data[3], data[2] }) - 4;
            }
            return length;
        }

        private List<byte> GetReadCommands(List<CommAddress> address, ref int index)
        {
            // List<SiemensAddress>每个内容都是一个Item
            List<byte[]> cmds = new List<byte[]>();

            List<byte> paramBytes = new List<byte>();
            int byteCount = 0;
            int itemCount = 0;
            for (; index < address.Count; index++)
            {
                var item = address[index] as SiemensAddress;
                if (item.ByteCount + 23 + byteCount < this._pduSize)
                {
                    itemCount++;
                    byteCount += item.ByteCount + 4;

                    List<byte> paramTemp = GetParamItemBytes(item);

                    paramBytes.AddRange(paramTemp);
                }
                else
                {
                    //paramBytes.Insert(0, (byte)itemCount);
                    //paramBytes.Insert(0, 0x04);
                    //cmds.Add(this.GetRequestCommands(paramBytes).ToArray());

                    //paramBytes.Clear();
                    //itemCount = 0;
                    //byteCount = 0;
                    //i--;

                    break;
                }
            }
            //if (paramBytes.Count > 0)
            //{
            //    paramBytes.Insert(0, (byte)itemCount);
            //    paramBytes.Insert(0, 0x04);
            //    cmds.Add(this.GetRequestCommands(paramBytes).ToArray());

            //}

            return this.GetRequestCommands(paramBytes);
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

        private Result Connect()
        {
            Result result = new Result();

            try
            {
                if (this.TransferObject == null) throw new Exception("通信组件不可用");

                // 打开TCP连接
                var prop = this.Props.FirstOrDefault(p => p.PropName == "TryCount");
                int tryCount = 30;
                if (prop != null)
                    int.TryParse(prop.PropValue, out tryCount);
                Result connectState = this.TransferObject.Connect(tryCount);
                if (!connectState.Status)
                    return connectState;

                // 发送COTP报文
                prop = this.Props.FirstOrDefault(p => p.PropName == "Rack");
                int rack = 0;
                if (prop != null)
                    int.TryParse(prop.PropValue, out tryCount);

                prop = this.Props.FirstOrDefault(p => p.PropName == "Slot");
                int slot = 0;
                if (prop != null)
                    int.TryParse(prop.PropValue, out tryCount);
                Result cotpState = COTPConnection(rack, slot);
                if (!cotpState.Status)
                    return cotpState;

                // 发送SetupCommunication报文 
                Result setupState = SetupCommunication();
                if (!setupState.Status)
                    return setupState;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }
        // COTP
        private Result COTPConnection(int rack, int slot)
        {
            Result result = new Result();

            List<byte> cotpBytes = new List<byte> {
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
                var prop = this.Props.FirstOrDefault(p => p.PropName == "Timeout");
                int timeout = 5000;
                if (prop != null)
                    int.TryParse(prop.PropValue, out timeout);


                Result<List<byte>> resp = this.TransferObject.SendAndReceived(cotpBytes, 4, timeout, this.CalcDataLength);
                //解析响应报文
                //byte[] respBytes = new byte[22];
                //int count = socket.Receive(respBytes, 0, 22, SocketFlags.None);
                if (resp.Data[5] != 0xd0)
                {
                    throw new Exception("COTP连接响应异常");
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "COTP连接未建立！" + ex.Message;
            }

            return result;
        }
        // Setup
        private Result SetupCommunication()
        {
            Result result = new Result();
            List<byte> setupBytes = new List<byte> {
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
                var prop = this.Props.FirstOrDefault(p => p.PropName == "Timeout");
                int timeout = 5000;
                if (prop != null)
                    int.TryParse(prop.PropValue, out timeout);

                Result<List<byte>> setp_result = this.TransferObject.SendAndReceived(setupBytes, 4, timeout, this.CalcDataLength);
                //socket.Send(setupBytes);
                //byte[] respBytes = new byte[27];
                //int count = socket.Receive(respBytes);
                // 拿到PDU长度   后续进行报文组装和接收的时候可以参考
                byte[] pdu_size = new byte[2];
                pdu_size[0] = setp_result.Data[26];
                pdu_size[1] = setp_result.Data[25];

                this._pduSize = BitConverter.ToInt16(pdu_size);
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Setup通信未建立！" + ex.Message;
            }
            return result;
        }
    }
}
