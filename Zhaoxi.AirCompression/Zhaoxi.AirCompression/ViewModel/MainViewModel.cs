using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Driver;
using Zhaoxi.AirCompression.Driver.Base;
using Zhaoxi.AirCompression.Model;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Zhaoxi.AirCompression.Components;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        IDeviceBLL _deviceBLL;
        Task task = null;
        CancellationTokenSource cancellation = new CancellationTokenSource();
        AutoResetEvent autoResetEvent = new AutoResetEvent(true);


        private FrameworkElement _currnetPage;
        public FrameworkElement CurrentPage
        {
            get { return _currnetPage; }
            set { Set(ref _currnetPage, value); }
        }

        public DeviceCollection Devices { get; set; }

        private string _temperatureDevice;

        public string TemperatureDevice
        {
            get { return _temperatureDevice; }
            set { Set(ref _temperatureDevice, value); }
        }

        private string _temperatureVarable;

        public string TemperatureVarable
        {
            get { return _temperatureVarable; }
            set { Set(ref _temperatureVarable, value); }
        }

        private string _humidityDevice;

        public string HumidityDevice
        {
            get { return _humidityDevice; }
            set { Set(ref _humidityDevice, value); }
        }
        private string _humidityVarable;

        public string HumidityVarable
        {
            get { return _humidityVarable; }
            set { Set(ref _humidityVarable, value); }
        }

        public ICommand SwitchPage
        {
            get => new RelayCommand<object>(obj =>
            {
                this.CurrentPage = this.GetPage(obj.ToString());
            });
        }


        public MainViewModel(IDeviceBLL deviceBLL)
        {
            _deviceBLL = deviceBLL;


            if (IsInDesignMode)
            {
            }
            else
            {
                // 获取设备列表信息
                Devices = new DeviceCollection(_deviceBLL.GetDevice());
                //Devices = new DeviceCollection();
                //Devices.Add(new DeviceModel { DeviceNum = "D01", DeviceName = "温湿度变送记录仪", Variables = new List<VariableModel> { new VariableModel { VariableNum = "V01", VariableDescription = "站房温度" }, new VariableModel { VariableNum = "V02", VariableDescription = "站房湿度" } }, State = 1 });

                //Devices.Add(new DeviceModel { DeviceName = "空压设备1#", State = 1 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备2#", State = 2 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备3#", State = 3 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备4#", State = 1 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备5#", State = 2 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备6#", State = 3 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备7#", State = 1 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备8#", State = 2 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备9#", State = 3 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备1#", State = 1 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备2#", State = 2 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备3#", State = 3 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备4#", State = 1 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备5#", State = 2 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备6#", State = 3 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备7#", State = 1 });
                //Devices.Add(new DeviceModel { DeviceName = "空压设备8#", State = 2 });

                // 获取温度配置信息
                ConfigModel configModel = _deviceBLL.GetConfig("TD01");
                if (configModel != null)
                {
                    TemperatureDevice = configModel.ConfigDevice;
                    TemperatureVarable = configModel.ConfigValue;
                }
                // 获取湿度配置信息
                configModel = _deviceBLL.GetConfig("HD01");
                if (configModel != null)
                {
                    HumidityDevice = configModel.ConfigDevice;
                    HumidityVarable = configModel.ConfigValue;
                }

                //Task.Run(async () =>
                //{
                //    await Task.Delay(2000);
                //    Devices["D01"]["V01"].CurrentValue = "50";
                //    Devices["D01"]["V02"].CurrentValue = "40";
                //    Application.Current.Dispatcher.Invoke(() =>
                //    {
                //        Devices.Add(new DeviceModel { DeviceName = "空压设备19#", State = 2 });
                //        Devices.Add(new DeviceModel { DeviceName = "空压设备11#", State = 2 });
                //    });
                //});
                this.DoMonitor();
            }
            Messenger.Default.Register<AlarmModel>(this, "AlarmNotification", DoAlarm);
            Messenger.Default.Register<UnionModel>(this, "UnionNotification", DoUnionControl);


            this.CurrentPage = this.GetPage("DevicePage");
        }
        private FrameworkElement GetPage(string pageName)
        {
            Type type = this.GetType().Assembly.GetType($"Zhaoxi.AirCompression.View.Pages.{pageName}");
            return (FrameworkElement)Activator.CreateInstance(type);
        }

        // Sqlite库
        // 数据库

        private void DoMonitor()
        {
            /// 拿到所有设备信息
            //var devices = _deviceBLL.GetDevice();
            // 每一台设备  用一个线程
            task = Task.Run(async () =>
            {
                Communication communication = null;
                //int index = 0;
                while (!cancellation.IsCancellationRequested)
                {
                    await Task.Delay(200);
                    autoResetEvent.WaitOne();

                    Model.DeviceModel device = Devices.Next();
                    //index++;
                    //if (index >= Devices.Count)
                    //    index = 0;

                    if (device == null ||
                        device.State == 0 ||
                        device.CommProperties == null ||
                        device.CommProperties.Count == 0 ||
                        device.Variables == null ||
                        device.Variables.Count == 0)
                        continue;


                    //获取点位数据
                    CommProperty commProperty = new CommProperty();
                    //propDic.Add("Protocol", Enum.Parse(typeof(Protocols), "ModbusAscii"));
                    // 反射通信配置信息
                    foreach (var item in device.CommProperties)
                    {
                        object v = null;
                        PropertyInfo pi = commProperty.GetType().GetProperty(item.PropName.Trim());
                        if (pi == null) continue;
                        Type propType = pi.PropertyType;
                        if (propType.IsEnum)
                        {
                            v = Enum.Parse(propType, item.PropValue.Trim() as string);
                        }
                        else
                        {
                            v = Convert.ChangeType(item.PropValue.Trim(), propType);
                        }
                        pi.SetValue(commProperty, v);
                    }
                    // 点位配置信息
                    List<PointProperty> points = new List<PointProperty>();
                    foreach (var variable in device.Variables)
                    {
                        Type type = Type.GetType(variable.VarType);// 根据类型字符串，获取对应的Type
                        int typeLen = Marshal.SizeOf(type);
                        points.Add(new PointProperty
                        {
                            PointId = variable.VariableNum,
                            Address = variable.VarAddress,// 地址字符串
                            ValueType = type, // 对应的数据类型
                            ByteCount = typeLen,
                            ValueBytes = new byte[typeLen]
                        });
                    }

                    communication = Communication.Create(commProperty);
                    communication.ReadAsync(points, result =>
                    {
                        if (result.IsSuccessed)
                        {
                            foreach (var point in points)
                            {
                                var value = communication?.ConvertValue(point.ValueBytes, point.ValueType);

                                var vItem = device.Variables.First(v => v.VariableNum == point.PointId);
                                if (vItem != null)
                                {
                                    // 偏移
                                    if (!string.IsNullOrEmpty(vItem.Operator) &&
                                     !string.IsNullOrEmpty(vItem.OffsetValue))
                                    {
                                        // 123 * 0.01
                                        string p = value.ToString() + vItem.Operator + vItem.OffsetValue;
                                        value = new DataTable().Compute(p, "");
                                    }
                                    // 拿到后终的数据
                                    // 如果报警与联控逻辑放在CurrentValue里做处理，可以做无限层级的连续控制
                                    // 如果放在外层，则不太好处理
                                    vItem.CurrentValue = value.ToString();
                                }
                                Debug.WriteLine(value);
                            }
                        }
                        else
                            device.CommErrorMessage = result.Message;

                        communication?.Dispose();
                        communication = null;
                        autoResetEvent.Set();
                    });
                }
                if (communication != null)
                    communication.Dispose();
            }, cancellation.Token);
        }

        private void DoAlarm(AlarmModel alarm)
        {
            Debug.WriteLine(alarm.AlarmDesc);
        }

        private void DoUnionControl(UnionModel unionModel)
        {
            // 获取设备信息（通信配置）
            DeviceModel deviceModel = _deviceBLL.GetDeviceAndComm(unionModel.ExeDeviceNum);
            // 将写入的点位
            VariableModel variableModel = _deviceBLL.GetVariableById(unionModel.ExeVarNum);
            // 将写入的值及类型
            Type type = Type.GetType(unionModel.InputType);
            dynamic value = Convert.ChangeType(unionModel.InputVar, type);

            // 实例化一个通信参数对象
            CommProperty commProperty = new CommProperty();
            foreach (var item in deviceModel.CommProperties)
            {
                object v = null;
                PropertyInfo pi = commProperty.GetType().GetProperty(item.PropName.Trim());
                if (pi == null) continue;
                Type propType = pi.PropertyType;
                if (propType.IsEnum)
                {
                    v = Enum.Parse(propType, item.PropValue.Trim() as string);
                }
                else
                {
                    v = Convert.ChangeType(item.PropValue.Trim(), propType);
                }
                pi.SetValue(commProperty, v);
            }
            Communication communication = Communication.Create(commProperty);
            // 点位配置信息
            List<PointProperty> write_points = new List<PointProperty>();
            write_points.Add(new PointProperty { Address = variableModel.VarAddress, ValueType = Type.GetType(unionModel.InputType), ValueBytes = communication.SwitchEndian(BitConverter.GetBytes(value)) });

            communication.WriteAsync(write_points, result =>
            {
                if (result.IsSuccessed)
                    variableModel.CurrentValue = value;
                else
                    deviceModel.CommErrorMessage = result.Message;
            });

        }

        public override void Cleanup()
        {
            cancellation.Cancel();
            task?.ConfigureAwait(false);
        }
    }
}
