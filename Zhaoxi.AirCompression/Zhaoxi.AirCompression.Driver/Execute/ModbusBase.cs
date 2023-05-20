using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Driver.Base;
using Zhaoxi.AirCompression.Driver.Base.Address;

namespace Zhaoxi.AirCompression.Driver.Execute
{
    public abstract class ModbusBase : ExecuteBase
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

        public ModbusAddress AnalysisAddress(PointProperty pp, bool isRead = true)
        {
            ModbusAddress ma = new ModbusAddress();
            // 40001     ModbusAddress{func=03,startAddr=0,count=1}
            // 40002     ModbusAddress{func=03,startAddr=1,count=1}
            // 40003     ModbusAddress{func=03,startAddr=2,count=1}

            // 40008.0     
            // 40008.1     

            // 根据数据类型计算所需的寄存器数量
            int typeLen = Marshal.SizeOf(pp.ValueType);
            ma.Length = typeLen / 2;    // 常规   特殊使用：字符串

            if (pp.Address.StartsWith("0"))
            {
                ma.FuncCode = isRead ? 01 : 05;
                ma.Length = 1;
            }
            else if (pp.Address.StartsWith("1"))
            {
                ma.FuncCode = 02;
                ma.Length = 1;
            }
            else if (pp.Address.StartsWith("3"))
                ma.FuncCode = 04;
            else if (pp.Address.StartsWith("4"))
                ma.FuncCode = isRead ? 03 : 16;

            // 起始地址
            string[] addStrArray = pp.Address.Split('.');
            ma.StartAddress = int.Parse(addStrArray[0].Substring(1)) - 1;
            if (addStrArray.Length > 1)
                ma.BitAddress = int.Parse(addStrArray[1]);

            return ma;
        }

        public List<ModbusAddress> GroupAddress(List<PointProperty> points)
        {
            // 每20个寄存一个组
            // ModbusAddress{func=03,startAddr=0,count=1};   40001 ushort     一条指令
            // ModbusAddress{func=03,startAddr=1,count=1};   40002 ushort     count1+count2 = count =3   - 4 
            // ModbusAddress{func=03,startAddr=3,count=2};                     
            // ModbusAddress{func=03,startAddr=10,count=1};                    count  = 3     ｛数据解析匹配｝
            // ModbusAddress{func=03,startAddr=43,count=1};  
            // ModbusAddress{func=03,startAddr=44,count=1};  
            // ModbusAddress{func=01,startAddr=0,count=1};  
            // ModbusAddress{func=01,startAddr=1,count=1};  


            // 待处理：1、地址非升序   2、线圈的处理

            List<ModbusAddress> address = new List<ModbusAddress>();
            ModbusAddress firstMa = new ModbusAddress();
            ModbusAddress lastMa = new ModbusAddress();
            foreach (var item in points)
            {
                var maCurrent = this.AnalysisAddress(item);
                // 判断是不必于当前组
                // 功能码要一样、
                if (maCurrent.FuncCode != firstMa.FuncCode)
                {
                    lastMa = maCurrent;
                    firstMa = new ModbusAddress(maCurrent); // 当前组的第一个
                    address.Add(firstMa);
                }
                else
                {
                    // 判断 地址相对当前组的第一个地址有没有超越20个寄存器
                    // 20是个参考值
                    // 
                    if (maCurrent.StartAddress - firstMa.StartAddress + maCurrent.Length - lastMa.Length > 20)
                    {
                        lastMa = maCurrent;
                        firstMa = new ModbusAddress(maCurrent);
                        address.Add(firstMa);
                    }
                    else
                    {
                        // 没有超越的情况下，把地址长度增加
                        // item:当前这个地址
                        // lastMa:上一次的地址信息
                        firstMa.Length += (maCurrent.StartAddress - lastMa.StartAddress + maCurrent.Length - lastMa.Length);
                    }
                }
                lastMa = maCurrent;
            }



            return address;
        }

        protected virtual byte[] GetReadCommand(ModbusAddress modbusAddress, byte slaveId)
        {
            // 01 03 00 00 00 0B
            // RTU:  + CRC
            // Ascii:  +LRC  -> Ascii
            // TCP:  TCP报头+
            List<byte> command = new List<byte>();
            command.Add(slaveId);// 从站地址
            command.Add((byte)modbusAddress.FuncCode);// 功能码
            command.Add(BitConverter.GetBytes(modbusAddress.StartAddress)[1]);
            command.Add(BitConverter.GetBytes(modbusAddress.StartAddress)[0]);
            command.Add(BitConverter.GetBytes(modbusAddress.Length)[1]);
            command.Add(BitConverter.GetBytes(modbusAddress.Length)[0]);
            return command.ToArray();
        }

        protected virtual byte[] GetWriteCommand(byte[] data, ModbusAddress address, byte slaveId)
        {
            //ModbusAddress ma = address as ModbusAddress;
            List<byte> command = new List<byte>();
            command.Add(slaveId);
            command.Add((byte)address.FuncCode);
            command.Add(BitConverter.GetBytes(address.StartAddress)[1]);
            command.Add(BitConverter.GetBytes(address.StartAddress)[0]);

            // 写单线圈的时候不需要额外处理，一个点位-》一个寄存器写入
            // 写多寄存器的时候，需要添加写入数量以及字节数字节    不太合适：float  两个寄存器
            if (address.FuncCode == 0x10)// 写多寄存器
            {
                // 写寄存器数量
                command.Add(BitConverter.GetBytes(data.Length / 2)[1]);
                command.Add(BitConverter.GetBytes(data.Length / 2)[0]);
                // 要写入寄存器的字节数
                command.Add((byte)data.Length);
            }
            command.AddRange(data);

            return command.ToArray();
        }
    }
}
