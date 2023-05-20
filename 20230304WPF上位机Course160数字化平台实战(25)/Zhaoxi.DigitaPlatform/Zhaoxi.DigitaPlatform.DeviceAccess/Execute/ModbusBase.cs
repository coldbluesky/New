using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Execute
{
    public abstract class ModbusBase : ExecuteObject
    {
        protected static Dictionary<int, string> Errors = new Dictionary<int, string>
        {
            { 0x01, "非法功能码"},
            { 0x02, "非法数据地址"},
            { 0x03, "非法数据值"},
            { 0x04, "从站设备故障"},
            { 0x05, "确认，从站需要一个耗时操作"},
            { 0x06, "从站忙"},
            { 0x08, "存储奇偶性差错"},
            { 0x0A, "不可用网关路径"},
            { 0x0B, "网关目标设备响应失败"},
        };

        public override Result<List<CommAddress>> GroupAddress(List<VariableProperty> variables)
        {
            Result<List<CommAddress>> result = new Result<List<CommAddress>>();
            result.Data = new List<CommAddress>();
            try
            {
                // N个打包地址
                List<ModbusAddress> address = new List<ModbusAddress>();
                // 针对Modbus地址   :string "40001" "40011"
                foreach (var item in variables)
                {
                    // 将"40001"地址解析成   03  01  02
                    // 目的将所有同类型功能码的地址整合在一起，不考虑长度
                    var result_addr = this.AnalysisAddress(item);
                    if (!result_addr.Status)
                        throw new Exception(result_addr.Message);

                    var maCurrent = result_addr.Data as ModbusAddress;
                    maCurrent.VariableId = item.VarId;
                    // 
                    // 
                    var addr = address.FirstOrDefault(a => a.FuncCode == maCurrent.FuncCode);
                    if (addr == null)
                    {
                        var ma = new ModbusAddress(maCurrent);
                        ma.Variables.Add(maCurrent);
                        address.Add(ma);

                    }
                    else
                    {
                        // 调整
                        /// 40010 - 1、40020 - 2、40024 - 2、40030 - 1、40015 - 2、40005 - 1、40001 - 1、40100 - 2、40200 - 1
                        /// start:9   len:1                        condition
                        /// start:9   len:12    : 19 - 9 + 2       19 > 9  &&  19+2>9+1
                        /// start:9   len:16    : 23 - 9 + 2       23 > 9  &&  23+2>9+12
                        /// start:9   len:21    : 29 - 9 + 1       29 > 9  &&  29+1>9+16
                        /// start:9   len:21    : None             14 > 9  &&  14+2>9+21   X
                        /// start:4   len:26    : 9 - 4 + 21       4  < 9
                        /// start:0   len:30    : 4 - 0 + 26       0  < 4
                        /// 
                        /// 1、找出同类功能码的
                        /// 2、进行起始地址排序
                        /// 3、最大地址-最小地址+最大地址的长度
                        /// 
                        /// 前提是将所有地址都转换出来
                        /// 这个处理方式，自行尝试！！！
                        /// 
                        if (maCurrent.StartAddress > addr.StartAddress)
                        {
                            if (maCurrent.StartAddress + maCurrent.Length > addr.StartAddress + addr.Length)
                                addr.Length = maCurrent.StartAddress - addr.StartAddress + maCurrent.Length;
                        }
                        else if (maCurrent.StartAddress < addr.StartAddress)
                        {
                            addr.Length += addr.StartAddress - maCurrent.StartAddress;
                            addr.StartAddress = maCurrent.StartAddress;
                        }

                        addr.Variables.Add(maCurrent);
                    }
                }
                address.ForEach(a => result.Data.Add(a));
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public override Result<CommAddress> AnalysisAddress(VariableProperty item, bool is_write = false)
        {
            Result<CommAddress> result = new Result<CommAddress>();
            try
            {
                ModbusAddress ma = new ModbusAddress();
                ma.VariableId = item.VarId;
                // 根据数据类型计算所需的寄存器数量
                int typeLen = Marshal.SizeOf(item.ValueType);
                ma.Length = typeLen / 2;    // 常规   特殊使用：字符串

                if (item.VarAddr.StartsWith("0"))
                {
                    ma.FuncCode = 01;
                    ma.Length = 1;
                    if (is_write)
                        ma.FuncCode = 15;
                }
                else if (item.VarAddr.StartsWith("1"))
                {
                    ma.FuncCode = 02;
                    ma.Length = 1;
                }
                else if (item.VarAddr.StartsWith("3"))
                    ma.FuncCode = 04;
                else if (item.VarAddr.StartsWith("4"))
                    ma.FuncCode = is_write ? 16 : 03;

                // 起始地址
                ma.StartAddress = int.Parse(item.VarAddr.Substring(1)) - 1;// 关于减1的动作，可以通过配置来确定

                result.Data = ma;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }
        //private ModbusAddress AnalysisAddress(VariableProperty item)
        //{
        //    ModbusAddress ma = new ModbusAddress();
        //    // 根据数据类型计算所需的寄存器数量
        //    int typeLen = Marshal.SizeOf(item.ValueType);
        //    ma.Length = typeLen / 2;    // 常规   特殊使用：字符串

        //    if (item.VarAddr.StartsWith("0"))
        //    {
        //        ma.FuncCode = 01;
        //        ma.Length = 1;
        //    }
        //    else if (item.VarAddr.StartsWith("1"))
        //    {
        //        ma.FuncCode = 02;
        //        ma.Length = 1;
        //    }
        //    else if (item.VarAddr.StartsWith("3"))
        //        ma.FuncCode = 04;
        //    else if (item.VarAddr.StartsWith("4"))
        //        ma.FuncCode = 03;

        //    // 起始地址
        //    ma.StartAddress = int.Parse(item.VarAddr.Substring(1)) - 1;// 关于减1的动作，可以通过配置来确定

        //    return ma;
        //}

        protected List<byte> CreateReadPDU(byte slaveNum, byte funcCode, ushort startAddr, ushort count)
        {
            List<byte> datas = new List<byte>();
            datas.Add(slaveNum);
            datas.Add(funcCode);

            datas.Add((byte)(startAddr / 256));
            datas.Add((byte)(startAddr % 256));

            datas.Add((byte)(count / 256));
            datas.Add((byte)(count % 256));

            return datas;
        }

        protected List<byte> CreateWritePDU(byte slaveNum, byte funcCode, ushort startAddr, byte[] data)
        {
            //ModbusAddress ma = address as ModbusAddress;
            List<byte> command = new List<byte>();
            command.Add(slaveNum);
            command.Add(funcCode);
            command.Add(BitConverter.GetBytes(startAddr)[1]);
            command.Add(BitConverter.GetBytes(startAddr)[0]);

            if (funcCode == 0x10)// 写多寄存器
            {
                // 写寄存器数量
                command.Add(BitConverter.GetBytes(data.Length / 2)[1]);
                command.Add(BitConverter.GetBytes(data.Length / 2)[0]);
                // 要写入寄存器的字节数
                command.Add((byte)data.Length);
            }
            command.AddRange(data);

            return command;
        }

        public override Result Read(List<CommAddress> variables)
        {
            Result result = new Result();
            try
            {
                var prop = this.Props.FirstOrDefault(p => p.PropName == "SlaveId");
                if (prop == null)
                    throw new Exception("未配置从站地址");

                byte slaveId = 0x01;
                byte.TryParse(prop.PropValue, out slaveId);

                foreach (ModbusAddress ma in variables)
                {
                    ushort max = 120;
                    int reqTotalCount = ma.Length;
                    if (ma.FuncCode == 0x01 || ma.FuncCode == 0x02)
                    {
                        max = 240 * 8;
                    }

                    List<byte> bytes = new List<byte>();
                    ushort startAddr = (ushort)ma.StartAddress;

                    for (ushort i = 0; i < reqTotalCount; i += max)
                    {
                        startAddr += i;
                        var perCount = (ushort)Math.Min(reqTotalCount - i, max);
                        var dataBytesLen = perCount * 2;
                        if (ma.FuncCode == 0x01 || ma.FuncCode == 0x02)
                            dataBytesLen = (int)Math.Ceiling(perCount * 1.0 / 8);

                        bytes.AddRange(this.Read(slaveId, (byte)ma.FuncCode, startAddr, perCount, (ushort)dataBytesLen));
                    }

                    // 寄存器
                    if (new int[] { 03, 04 }.Contains(ma.FuncCode))
                    {
                        // 需要知道变量列表   地址长度
                        //item.Addresses[0].StartAddress
                        //item.Addresses[0].Length
                        for (int i = 0; i < ma.Variables.Count; i++)
                        {
                            var addr = ma.Variables[i] as ModbusAddress;
                            var start = addr.StartAddress - ma.StartAddress;
                            var len = addr.Length * 2;
                            byte[] dataBytes = bytes.GetRange(start * 2, len).ToArray();

                            addr.ValueBytes = this.SwitchEndianType(new List<byte>(dataBytes)).ToArray();
                        }
                    }
                    // 线圈  Byte->8个状态 
                    else if (new int[] { 01, 02 }.Contains(ma.FuncCode))
                    {
                        // 状态全部转出来   
                        // 根据位置进行结果获取   0x00  0x01
                        List<byte> resultState = new List<byte>();
                        bytes.ForEach(b =>
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                resultState.Add((byte)((b & (1 << i)) >> i));
                            }
                        });
                        for (int i = 0; i < ma.Variables.Count; i++)
                        {
                            var addr = ma.Variables[i] as ModbusAddress;
                            var start = addr.StartAddress - ma.StartAddress;
                            addr.ValueBytes = resultState.GetRange(start, 1).ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }

        protected virtual List<byte> Read(byte slaveNum, byte funcCode, ushort startAddr, ushort count, ushort respLen)
        {
            return null;
        }

        public override Result Write(List<CommAddress> addresses)
        {
            Result result = new Result();
            try
            {
                var prop = this.Props.FirstOrDefault(p => p.PropName == "SlaveId");
                if (prop == null)
                    throw new Exception("未配置从站地址");

                byte slaveId = 0x01;
                byte.TryParse(prop.PropValue, out slaveId);


                foreach (ModbusAddress item in addresses)
                {
                    this.Write(slaveId, (byte)item.FuncCode, (ushort)item.StartAddress, item.ValueBytes, 8);
                }
            }
            catch (Exception ex)
            {
                result = new Result(false, ex.Message);
            }

            return result;
        }

        protected virtual void Write(byte slaveNum, byte funcCode, ushort startAddr, byte[] data, ushort respLen) { }
    }
}
