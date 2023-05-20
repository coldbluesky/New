using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Driver.Base;
using Zhaoxi.AirCompression.Driver.Component;
using Zhaoxi.AirCompression.Driver.Execute;

namespace Zhaoxi.AirCompression.Driver
{
    public class Communication : IDisposable
    {
        private IComponent _component;
        private ExecuteBase _execute;

        public CommProperty CommProperty { get; set; }

        // 有可能多个通信对象，也有可能只有一个对象
        //public Communication(CommProperty commProperty)
        //{
        //    _commProperty = commProperty;

        //    if ((int)commProperty.Protocol < 0x80)
        //    {
        //        _component = new ComponentSerial(commProperty);
        //    }
        //    else
        //    {
        //        _component = new ComponentSocket(commProperty);
        //    }


        //    // component -》 执行过程 (各种协议的执行过程，准备报文     数据解析)
        //    //component.SendAndReceive()
        //}

        // 实际使用中，并不完全是一个对象，有可能是多个通信对象
        private static List<Communication> _instanceList = new List<Communication>();
        private static readonly object Singleton_lock = new object();
        private Communication(CommProperty commProperty)
        {
            this.CommProperty = commProperty;

            if ((int)commProperty.Protocol < 0x80)
            {
                _component = new ComponentSerial(commProperty);
            }
            else
            {
                _component = new ComponentSocket(commProperty);
            }

            Type type = this.GetType().Assembly.GetType("Zhaoxi.AirCompression.Driver.Execute." + commProperty.Protocol.ToString());
            _execute = (ExecuteBase)Activator.CreateInstance(type);
        }
        public static Communication Create(CommProperty commProperty)
        {
            var instance = _instanceList.FirstOrDefault(il => il.CommProperty.Compare(commProperty));
            if (instance == null)
            {
                lock (Singleton_lock)
                {
                    if (instance == null)
                        instance = new Communication(commProperty);
                    _instanceList.Add(instance);
                }
            }
            return instance;
        }


        public Result Read(List<PointProperty> points)
        {
            // 通过不同的协议，将points转换成对应的指令byte[]
            // 40001   funccode=3,startAddr=0,count=1
            // 40002   funccode=3,startAddr=1,count=1
            // 40003   funccode=3,startAddr=2,count=1
            // 40013   funccode=3,startAddr=12,count=1  Modbus类型 /   S7类型

            // 40043   funccode=3,startAddr=0,count=1

            // 单个请求？请求的频率高，效率低
            // 连续请求？报文超长？分组（需要在不同的执行实例里面处理）    KepServer    （20个寄存器）

            // 根据不同的协议   实例化对应的执行对象 

            try
            {
                //Type type = this.GetType().Assembly.GetType("Zhaoxi.AirCompression.Driver.Execute." + this.CommProperty.Protocol.ToString());
                //// 1、创建的过程中，只有通信组件对象
                //ExecuteBase executeBase = (ExecuteBase)Activator.CreateInstance(type);
                if (_execute == null) return new Result(false, "通信组件初始化异常！");

                // 2、连接
                Result connect_result = _component.Connect().GetAwaiter().GetResult();
                if (!connect_result.IsSuccessed) return connect_result;

                // 3、部分协议需要：S7-    CTOP   SetupCommunication
                Result build_result = _execute.BuildCommunication(_component, this.CommProperty);
                if (!build_result.IsSuccessed) return build_result;

                // 4、
                Result read_result = _execute.Read(points, _component, this.CommProperty);

                return read_result;
            }
            catch (Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }
        public void ReadAsync(List<PointProperty> points, Action<Result> callback)
        {
            if (_component == null || _execute == null)
                callback?.Invoke(new Result(false, "通信组件初始化异常！"));

            Result result = new Result();

            try
            {
                // 通信组件建立连接
                Result connect_result = _component.Connect().GetAwaiter().GetResult();
                if (!connect_result.IsSuccessed)
                    callback?.Invoke(connect_result);

                // 建立必要的通信连接（部分协议需要）
                Result build_result = _execute.BuildCommunication(_component, this.CommProperty);
                if (!build_result.IsSuccessed)
                    callback?.Invoke(build_result);


                // 执行读取操作
                // 地址分组(地址解析)---生成Command---解析byte[]返回数据
                _execute.ReadAsync(points, _component, this.CommProperty, callback);
            }
            catch (Exception ex)
            {
                callback?.Invoke(new Result(false, ex.Message));
            }
        }

        public Result Write(List<PointProperty> points)
        {
            Result result = new Result();

            try
            {
                //Type type = this.GetType().Assembly.GetType("Zhaoxi.AirCompression.Driver.Execute." + this.CommProperty.Protocol.ToString());
                //// 1、创建的过程中，只有通信组件对象
                //ExecuteBase executeBase = (ExecuteBase)Activator.CreateInstance(type);
                if (_execute == null) return new Result(false, "通信组件初始化异常！");


                // 通信组件建立连接
                Result connect_result = _component.Connect().GetAwaiter().GetResult();
                if (!connect_result.IsSuccessed) return connect_result;
                // 建立必要的通信连接（部分协议需要）
                Result build_result = _execute.BuildCommunication(_component, this.CommProperty);
                if (!build_result.IsSuccessed) return build_result;


                // 执行写入操作
                // 地址分组(地址解析)---生成Command---解析byte[]返回数据
                return _execute.Write(points, _component, this.CommProperty);
            }
            catch (Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }

        public void WriteAsync(List<PointProperty> points, Action<Result> callback)
        {
            if (_component == null || _execute == null)
                callback?.Invoke(new Result(false, "通信组件初始化异常！"));

            Result result = new Result();

            try
            {
                // 通信组件建立连接
                Result connect_result = _component.Connect().GetAwaiter().GetResult();
                if (!connect_result.IsSuccessed)
                    callback?.Invoke(connect_result);

                // 建立必要的通信连接（部分协议需要）
                Result build_result = _execute.BuildCommunication(_component, this.CommProperty);
                if (!build_result.IsSuccessed)
                    callback?.Invoke(build_result);


                // 执行写入操作
                // 地址分组(地址解析)---生成Command---解析byte[]返回数据
                _execute.WriteAsync(points, _component, this.CommProperty, callback);
            }
            catch (Exception ex)
            {
                callback?.Invoke(new Result(false, ex.Message));
            }
        }

        public T ConvertValue<T>(byte[] valueBytes)
        {
            dynamic value = default(T);
            if (typeof(T) == typeof(bool))
            {
                value = valueBytes[0] == 0x01;
            }
            else if (typeof(T) == typeof(string))
            {
                value = Encoding.UTF8.GetString(valueBytes);
            }
            else
            {
                valueBytes = SwitchEndian(valueBytes);

                //T value = default(T);
                Type tBitConverter = typeof(BitConverter);
                MethodInfo method = tBitConverter.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(mi => mi.ReturnType == typeof(T)) as MethodInfo;
                if (method == null)
                    throw new Exception("未找到匹配的数据类型转换方法");

                value = method?.Invoke(tBitConverter, new object[] { valueBytes.ToArray(), 0 });
            }

            return value;
        }

        public object ConvertValue(byte[] valueBytes, Type type)
        {
            dynamic value;
            if (type == typeof(bool))
            {
                value = valueBytes[0] == 0x01;
            }
            else if (type == typeof(string))
            {
                value = Encoding.UTF8.GetString(valueBytes);
            }
            else
            {
                valueBytes = SwitchEndian(valueBytes);

                //T value = default(T);
                Type tBitConverter = typeof(BitConverter);
                MethodInfo method = tBitConverter.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(mi => mi.ReturnType == type) as MethodInfo;
                if (method == null)
                    throw new Exception("未找到匹配的数据类型转换方法");

                value = method?.Invoke(tBitConverter, new object[] { valueBytes.ToArray(), 0 });
            }

            return value;
        }

        public byte[] SwitchEndian(byte[] value)
        {
            List<byte> result = new List<byte>(value);
            switch (CommProperty.EndianType)
            {
                case EndianType.AB:
                case EndianType.ABCD:
                case EndianType.ABCDEFGH:
                    result.Reverse();
                    return result.ToArray();
                case EndianType.CDAB: // 4字节处理
                    if (value.Length == 4)
                    {
                        result[3] = value[2];
                        result[2] = value[3];
                        result[1] = value[0];
                        result[0] = value[1];
                    }
                    return result.ToArray();
                case EndianType.BADC: // 4字节处理
                    if (value.Length == 4)
                    {
                        result[3] = value[1];
                        result[2] = value[0];
                        result[1] = value[3];
                        result[0] = value[2];
                    }
                    return result.ToArray();
                case EndianType.GHEFCDAB:  // 8字节处理
                    if (value.Length == 8)
                    {
                        result[7] = value[6];
                        result[6] = value[7];
                        result[5] = value[4];
                        result[4] = value[5];
                        result[3] = value[2];
                        result[2] = value[3];
                        result[1] = value[0];
                        result[0] = value[1];
                    }
                    return result.ToArray();
                case EndianType.BADCFEHG: // 8字节处理
                    if (value.Length == 8)
                    {
                        result[7] = value[1];
                        result[6] = value[0];
                        result[5] = value[3];
                        result[4] = value[2];
                        result[3] = value[5];
                        result[2] = value[4];
                        result[1] = value[7];
                        result[0] = value[6];
                    }
                    return result.ToArray();
                case EndianType.BA:
                case EndianType.DCBA:
                case EndianType.HGFEDCBA:
                default:
                    return value;
            }
        }

        public void Dispose()
        {
            //_component.Close();
            _instanceList.RemoveAll(il => il.CommProperty.Compare(this.CommProperty) && il._component.Close().IsSuccessed);
        }
    }
}
