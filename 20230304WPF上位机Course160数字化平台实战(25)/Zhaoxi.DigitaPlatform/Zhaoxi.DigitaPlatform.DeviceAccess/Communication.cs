using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Execute;
using Zhaoxi.DigitaPlatform.DeviceAccess.Transfer;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess
{
    public class Communication
    {
        private static Communication _instace;
        private static object _lock = new object();
        private Communication() { }

        public static Communication Create()
        {
            if (_instace == null)
            {
                lock (_lock)
                {
                    if (_instace == null)
                        _instace = new Communication();
                }
            }
            return _instace;
        }


        private List<TransferObject> TransferList = new List<TransferObject>();

        public Result<ExecuteObject> GetExecuteObject(List<DevicePropItemEntity> props)
        {
            Result<ExecuteObject> result = new Result<ExecuteObject>();

            try
            {
                // ����ִ�е�Ԫ
                var protocol = props.FirstOrDefault(p => p.PropName == "Protocol");
                if (protocol == null)
                {
                    throw new Exception("Э����Ϣδ֪");
                }

                Type type = Assembly.Load("Zhaoxi.DigitaPlatform.DeviceAccess")
                           .GetType("Zhaoxi.DigitaPlatform.DeviceAccess.Execute." + protocol.PropValue);
                if (type == null)
                {
                    // �����쳣
                    throw new Exception("ִ�ж���������Ч");
                }
                ExecuteObject eo = Activator.CreateInstance(type) as ExecuteObject;
                if (eo == null)
                {
                    // �����쳣
                    throw new Exception("ִ�ж��󴴽�ʧ��");
                }

                var r1 = eo.Match(props, TransferList);
                if (!r1.Status)
                {
                    result.Status = false;
                    result.Message = r1.Message;
                }
                else
                    result.Data = eo;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public Result<object> ConvertType(byte[] valueBytes, Type type)
        {
            Result<object> result = new Result<object>();

            try
            {
                if (type == typeof(bool))
                {
                    result.Data = valueBytes[0] == 0x01;
                }
                else if (type == typeof(string))
                {
                    result.Data = Encoding.UTF8.GetString(valueBytes);
                }
                else
                {
                    // ���ﲻ��Ҫ�ֽڵ���
                    Type tBitConverter = typeof(BitConverter);
                    MethodInfo method = tBitConverter.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(mi => mi.ReturnType == type && mi.GetParameters().Length == 2) as MethodInfo;
                    if (method == null)
                        throw new Exception("δ�ҵ�ƥ�����������ת������");

                    result.Data = method?.Invoke(tBitConverter, new object[] { valueBytes.ToArray(), 0 });
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
