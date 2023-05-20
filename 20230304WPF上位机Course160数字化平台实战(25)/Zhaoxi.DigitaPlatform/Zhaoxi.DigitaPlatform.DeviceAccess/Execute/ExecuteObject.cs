using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.Common.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Transfer;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Execute
{
    public abstract class ExecuteObject
    {
        // 动态配置字节
        public EndianType EndianType { get; set; } = EndianType.ABCD;

        internal List<DevicePropItemEntity> Props { get; set; }

        internal TransferObject TransferObject { get; set; }

        internal Result Match(List<DevicePropItemEntity> props, List<TransferObject> tos, List<string> conditions, string protocol)
        {
            Result result = new Result();

            try
            {
                this.Props = props;

                var prop = props.FirstOrDefault(p => p.PropName == "Endian");
                if (prop != null)
                    this.EndianType = (EndianType)Enum.Parse(typeof(EndianType), prop.PropValue);

                // 从tos列表中找到PortName值一样的对象
                this.TransferObject = tos.FirstOrDefault(
                    to =>
                    to.GetType().Name == protocol &&
                    conditions.All(s => to.Conditions.Any(c => c == s))  // 匹配两上集合是否一致
                    );

                if (this.TransferObject == null)
                {
                    Type type = this.GetType().Assembly.GetType("Zhaoxi.DigitaPlatform.DeviceAccess.Transfer." + protocol);
                    this.TransferObject = (TransferObject)Activator.CreateInstance(type);

                    this.TransferObject.Conditions = conditions;
                    tos.Add(this.TransferObject);

                    // 初始化相关属性   
                    Result result_config = this.TransferObject.Config(props);
                    if (!result_config.Status)
                        return result_config;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }
        internal virtual Result Match(List<DevicePropItemEntity> props, List<TransferObject> tos)
            => new Result();


        public virtual void Connect() { }
        public virtual Result Read(List<CommAddress> variables) => new Result();
        public virtual void ReadAsync() { }
        public virtual Result Write(List<CommAddress> addresses) => new Result();
        public virtual void WriteAsync() { }

        public virtual Result Dispose()
        {
            if (this.TransferObject == null) return new Result();
            try
            {
                this.TransferObject?.Close();
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }



        public virtual Result<List<CommAddress>> GroupAddress(List<VariableProperty> variables)
        {
            return new Result<List<CommAddress>>(false, "");
        }
        public virtual Result<CommAddress> AnalysisAddress(VariableProperty item, bool is_write = false)
        {
            return new Result<CommAddress>(false, "");
        }




        /// <summary>
        /// 表示将一个数据字节进行指定字节序的调整
        /// </summary>
        /// <param name="bytes">接收待转换的设备中返回的字节数组</param>
        /// <returns>返回调整完成的字节数组</returns>
        public List<byte> SwitchEndianType(List<byte> bytes)
        {
            // 不管是什么字节序，这个Switch里返回的是ABCD这个顺序
            List<byte> temp = new List<byte>();
            switch (EndianType)  // alt+enter
            {
                case EndianType.ABCD:
                    temp = bytes;
                    break;
                case EndianType.DCBA:
                    for (int i = bytes.Count - 1; i >= 0; i--)
                    {
                        temp.Add(bytes[i]);
                    }
                    break;
                case EndianType.CDAB:
                    temp = new List<byte> { bytes[2], bytes[3], bytes[0], bytes[1] };
                    break;
                case EndianType.BADC:
                    temp = new List<byte> { bytes[1], bytes[0], bytes[3], bytes[2] };
                    break;
            }
            if (BitConverter.IsLittleEndian)
                temp.Reverse();

            return temp;
        }
    }
}
