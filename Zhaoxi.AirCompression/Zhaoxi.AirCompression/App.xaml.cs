using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.DAL;
using Zhaoxi.AirCompression.Driver;
using Zhaoxi.AirCompression.Driver.Base;
using Zhaoxi.AirCompression.View;

namespace Zhaoxi.AirCompression
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SimpleIoc.Default.Register<IDeviceDataAccess, DeviceDataAccess>();
            SimpleIoc.Default.Register<ILoginDataAccess, LoginDataAccess>();
            SimpleIoc.Default.Register<IRecordDataAccess, RecordDataAccess>();

            SimpleIoc.Default.Register<IDeviceBLL, DeviceBLL>();
            SimpleIoc.Default.Register<ILoginBLL, LoginBLL>();
            SimpleIoc.Default.Register<IAlarmBLL, AlarmBLL>();
            SimpleIoc.Default.Register<IReportBLL, ReportBLL>();



            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //using (var client = new HttpClient())
            //{
            //    var resp = client.GetAsync("http://localhost:5000/api/values/monitor").GetAwaiter().GetResult();
            //    var value = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //}
            //stopwatch.Stop();
            //Debug.WriteLine(stopwatch.ElapsedMilliseconds);




            //BaseTest();

            //Test();

            // 结果有一个执行不了
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (new LoginWindow().ShowDialog() == true)
            {
                new MainWindow().ShowDialog();
            }
            Application.Current.Shutdown(0);


            //new MainWindow().Show();
            //new Lo





        }

        private static void Test()
        {
            // 通信配置信息
            Dictionary<string, object> propDic = new Dictionary<string, object>();
            propDic.Add("Protocol", Enum.Parse(typeof(Protocols), "S7Net"));
            propDic.Add("IP", "192.168.2.1");
            propDic.Add("Port", 102);
            CommProperty commProperty = new CommProperty();
            // 反射
            foreach (var item in propDic)
            {
                commProperty.GetType().GetProperty(item.Key).SetValue(commProperty, item.Value);
            }

            // 监控点位信息
            List<string[]> points = new List<string[]>();
            #region Modbus Address
            //points.Add(new string[] { "40001", "System.UInt16" });   // 保持寄存器 0 -1
            //points.Add(new string[] { "40002", "System.UInt16" });   // 保持寄存器 1 -1
            //points.Add(new string[] { "40004", "System.Single" });   // 保持寄存器 3 -2
            //points.Add(new string[] { "40011", "System.UInt16" });   // 保持寄存器 10-1
            //points.Add(new string[] { "40044", "System.UInt16" });   // 保持寄存器 43-1
            //points.Add(new string[] { "40045", "System.Single" });   // 保持寄存器 44-2
            //points.Add(new string[] { "00001", "System.Boolean" });  // 线圈状态   0 -1
            //points.Add(new string[] { "00002", "System.Boolean" });  // 线圈状态   1 -1
            //points.Add(new string[] { "00006", "System.Boolean" });  // 线圈状态   1 -1   1 1 0 0 0 0 0 0
            //points.Add(new string[] { "00009", "System.Boolean" });  // 线圈状态   1 -1   1 1 0 0 0 0 0 0
            #endregion

            #region Siemens Address
            points.Add(new string[] { "DB1.6", "System.UInt16" });
            points.Add(new string[] { "DB1.8", "System.UInt16" });
            points.Add(new string[] { "DB1.10", "System.Int16" });// -123
            points.Add(new string[] { "DB1.18", "System.Single" });// Float
            points.Add(new string[] { "DB1.28.0", "System.Boolean" });//  1
            points.Add(new string[] { "DB1.28.1", "System.Boolean" });//  0
            points.Add(new string[] { "DB1.28.2", "System.Boolean" });//  1
            points.Add(new string[] { "DB1.50", "System.UInt16" });//
            points.Add(new string[] { "I0.0", "System.Boolean" });//   0
            points.Add(new string[] { "I0.1", "System.Boolean" });//   1
            points.Add(new string[] { "Q0.1", "System.Boolean" });//   1

            #endregion

            // 从数据库到地址集合
            List<PointProperty> pointProps = new List<PointProperty>();
            foreach (var item in points)
            {
                Type type = Type.GetType(item[1]);// 根据类型字符串，获取对应的Type
                int typeLen = Marshal.SizeOf(type);
                pointProps.Add(new PointProperty
                {
                    Address = item[0],// 地址字符串
                    ValueType = type, // 对应的数据类型
                    ByteCount = typeLen,
                    ValueBytes = new byte[typeLen]
                });
            }

            // 
            for (int i = 0; i < 100; i++)
            {
                Communication communication = Communication.Create(commProperty);
                var read_result = communication.Read(pointProps);
                if (read_result.IsSuccessed)
                {
                    foreach (var point in pointProps)
                    {
                        Debug.WriteLine(communication.ConvertValue(point.ValueBytes, point.ValueType));
                    }
                }
                else
                    Debug.WriteLine(read_result.Message);
                communication.Dispose();
            }
        }

        private static void BaseTest()
        {
            Dictionary<string, object> propDic = new Dictionary<string, object>();
            //propDic.Add("Protocol", Enum.Parse(typeof(Protocols), "ModbusAscii"));
            //propDic.Add("PortName", "COM10");
            //propDic.Add("BaudRate", 19200);

            CommProperty commProperty = new CommProperty();
            // 反射
            foreach (var item in propDic)
            {
                commProperty.GetType().GetProperty(item.Key).SetValue(commProperty, item.Value);
            }
            // 点位信息
            // DataTable
            List<string[]> points = new List<string[]>();
            points.Add(new string[] { "40001", "System.UInt16" });   // 保持寄存器 0 -1
            points.Add(new string[] { "40002", "System.UInt16" });   // 保持寄存器 1 -1
            points.Add(new string[] { "40004", "System.Single" });   // 保持寄存器 3 -2
            points.Add(new string[] { "40011", "System.UInt16" });   // 保持寄存器 10-1
            points.Add(new string[] { "40044", "System.UInt16" });   // 保持寄存器 43-1
            points.Add(new string[] { "40045", "System.Single" });   // 保持寄存器 44-2
            points.Add(new string[] { "00001", "System.Boolean" });  // 线圈状态   0 -1
            points.Add(new string[] { "00002", "System.Boolean" });  // 线圈状态   1 -1
            points.Add(new string[] { "00006", "System.Boolean" });  // 线圈状态   1 -1   1 1 0 0 0 0 0 0
            points.Add(new string[] { "00009", "System.Boolean" });  // 线圈状态   1 -1   1 1 0 0 0 0 0 0
            // 20个寄存器一次请求
            // 3个报文
            // 01 03 00 00 00 0B CRC
            // 01 03 00 2B 00 03 CRC
            // 01 01 00 00 00 02 CRC

            // 根据类型字符串得到   Type，根据Type，计算出对应的字节数
            // 01 03 00 00 00 01
            // 功能码/起始地址/寄存器数量

            List<PointProperty> pointProps = new List<PointProperty>();
            foreach (var item in points)
            {
                Type type = Type.GetType(item[1]);// 根据类型字符串，获取对应的Type
                int typeLen = Marshal.SizeOf(type);
                pointProps.Add(new PointProperty
                {
                    Address = item[0],// 地址字符串
                    ValueType = type, // 对应的数据类型
                    ByteCount = typeLen,
                    ValueBytes = new byte[typeLen]
                });
            }

            Communication communication = Communication.Create(commProperty);
            communication.ReadAsync(pointProps, resul =>
            {
                if (resul.IsSuccessed)
                {
                    foreach (var point in pointProps)
                    {
                        Debug.WriteLine(communication.ConvertValue(point.ValueBytes, point.ValueType));
                    }
                }
                else
                    Debug.WriteLine(resul.Message);
            });
            //var read_result = communication.Read(pointProps);
            //if (read_result.IsSuccessed)
            //{
            //    // 结果反馈
            //    // 输出最终的读出来的数据

            //    foreach (var point in pointProps)
            //    {
            //        //point.ValueBytes    最终数据字节
            //        //point.ValueType     转换成什么类型的数据
            //        //Type 实例    通过动态类型创建泛型方法
            //        //communication.ConvertValue<point.ValueType>(null);
            //        //var value = communication.GetType()
            //        //      .GetMethod("ConvertValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            //        //      .MakeGenericMethod(point.ValueType)
            //        //      .Invoke(communication, new object[] { point.ValueBytes });

            //        //Debug.WriteLine(value);

            //        Debug.WriteLine(communication.ConvertValue(point.ValueBytes, point.ValueType));
            //    }
            //}
            //else
            //    Debug.WriteLine(read_result.Message);

            //Communication communication_write = Communication.Create(commProperty);
            ushort v1 = 123;
            float v2 = 4.5f;
            List<PointProperty> write_points = new List<PointProperty>();
            write_points.Add(new PointProperty
            {
                Address = "40021",
                ValueType = typeof(ushort),
                ValueBytes = communication.SwitchEndian(BitConverter.GetBytes(v1))
            });
            write_points.Add(new PointProperty
            {
                Address = "40023",
                ValueType = typeof(float),
                ValueBytes = communication.SwitchEndian(BitConverter.GetBytes(v2))
            });
            communication.WriteAsync(write_points, result =>
            {
                if (result.IsSuccessed)
                {
                    Debug.WriteLine("写入成功！");
                }
                else
                {
                    Debug.WriteLine(result.Message);
                }
            });
        }


        // 同时读写的设想：
        // [R1 , R2 , R3 ,....]......    同个串口    D1  COM1   D2  COM2
        // [W1 , W2]  D3  COM1   
        // 名称一至   两种情况：相同串口和不同串口的操作
        // List<byte[]> {com1,com2,com3}


        // ModbusRTU.ReadAsync(......,.,,.,,   
        //var write_result = communication.Write(write_points);
        //if (write_result.IsSuccessed)
        //{
        //    Debug.WriteLine("写入成功！");
        //}
        //else
        //{
        //    Debug.WriteLine(write_result.Message);
        //}

        //communication.Dispose();
    }
}
