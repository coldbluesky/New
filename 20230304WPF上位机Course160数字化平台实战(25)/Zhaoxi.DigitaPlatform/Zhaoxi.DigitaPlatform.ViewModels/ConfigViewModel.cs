using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Zhaoxi.DigitaPlatform.Common;
using Zhaoxi.DigitaPlatform.Common.Base;
using Zhaoxi.DigitaPlatform.DataAccess;
using Zhaoxi.DigitaPlatform.Entities;
using Zhaoxi.DigitaPlatform.IDataAccess;
using Zhaoxi.DigitaPlatform.Models;

namespace Zhaoxi.DigitaPlatform.ViewModels
{
    /// <summary>
    /// 对应窗口的业务逻辑
    /// </summary>
    public class ConfigViewModel : ViewModelBase
    {
        public ObservableCollection<DeviceItemModel> DeviceList { get; set; } = new ObservableCollection<DeviceItemModel>();
        public List<PropOptionModel> PropOptions { get; set; }

        private DeviceItemModel currentDevice;
        public DeviceItemModel CurrentDevice
        {
            get => currentDevice;
            set => Set(ref currentDevice, value);
        }

        private string _saveFailedMessage;

        public string SaveFailedMessage
        {
            get { return _saveFailedMessage; }
            set { Set(ref _saveFailedMessage, value); }
        }


        public List<ThumbModel> ThumbList { get; set; }

        public RelayCommand<object> ItemDropCommand { get; set; }
        public RelayCommand<object> KeyDownCommand { get; set; }
        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand<object> CloseSaveFailedCommand { get; set; }
        public RelayCommand<DeviceItemModel> DeviceSelectedCommand { get; set; }

        public RelayCommand AlarmConditionCommand { get; set; }
        public RelayCommand UnionConditionCommand { get; set; }

        private bool _windowState = false;
        public RelayCommand<object> CloseCommand { get; set; }

        ILocalDataAccess _localDataAccess;

        public ConfigViewModel(ILocalDataAccess localDataAccess)
        {
            _localDataAccess = localDataAccess;

            if (!IsInDesignMode)
            {
                ThumbList = new List<ThumbModel>();
                // 通过数据库维护
                var ts = localDataAccess.GetThumbList();
                ThumbList = ts.GroupBy(t => t.Category).Select(g => new ThumbModel
                {
                    Header = g.Key,
                    Children = g.Select(b => new ThumbItemModel
                    {
                        Icon = "pack://application:,,,/Zhaoxi.DigitaPlatform.Assets;component/Images/Thumbs/" + b.Icon,
                        Header = b.Header,
                        TargetType = b.TargetType,
                        Width = b.Width,
                        Height = b.Height
                    }).ToList()
                }).ToList();
                // 初始化组态组件
                ComponentsInit();

                ItemDropCommand = new RelayCommand<object>(DoItemDropCommand);
                SaveCommand = new RelayCommand<object>(DoSaveCommand);
                CloseSaveFailedCommand = new RelayCommand<object>(obj =>
                {
                    VisualStateManager.GoToElementState(obj as Window, "SaveFailedClose", true);
                });

                DeviceSelectedCommand = new RelayCommand<DeviceItemModel>(model =>
                {
                    // 记录一个当前选中组件
                    // Model = DeviceItemModel
                    // 对当前组件进行选中
                    // 进行属性、点位编辑
                    if (CurrentDevice != null)
                        CurrentDevice.IsSelected = false;
                    if (model != null)
                    {
                        model.IsSelected = true;
                    }

                    CurrentDevice = model;
                });

                KeyDownCommand = new RelayCommand<object>(obj =>
                {
                    if (CurrentDevice != null)
                    {
                        var ea = obj as KeyEventArgs;
                        if (ea.Key == Key.Up)
                            CurrentDevice.Y--;
                        else if (ea.Key == Key.Down)
                            CurrentDevice.Y++;
                        else if (ea.Key == Key.Left)
                            CurrentDevice.X--;
                        else if (ea.Key == Key.Right)
                            CurrentDevice.X++;

                        else if (ea.Key == Key.Escape)
                            CurrentDevice.IsSelected = false;
                    }
                });

                CloseCommand = new RelayCommand<object>(obj =>
                {
                    (obj as Window).DialogResult = _windowState;
                });

                AlarmConditionCommand = new RelayCommand(() =>
                {
                    if (CurrentDevice != null)
                        ActionManager.Execute("AlarmCondition", CurrentDevice);
                });
                UnionConditionCommand = new RelayCommand(() =>
                {
                    if (CurrentDevice != null)
                        ActionManager.Execute("UnionCondition", CurrentDevice);
                });



                // 初始化组件通信属性选项
                var pos = localDataAccess.GetPropertyOption();
                PropOptions = pos.Select(p =>
                {
                    var pom = new PropOptionModel
                    {
                        Header = p.PropHeader,
                        PropName = p.PropName,
                        PropType = p.PropType
                    };
                    // 修改目的有两个：
                    // 1、初始化当前属性选项所对应的值的选项
                    // 2、希望加载值选项后，初始化一个默认选项
                    var list = InitOptions(p.PropName, out int DefaultIndex);
                    pom.ValueOptions = list;
                    pom.DefaultIndex = DefaultIndex;

                    return pom;
                }
                ).ToList();
            }

            //ActionManager.Register<string>("Key", new Action<string>(test));

            ActionManager.Register<Action<Func<List<DeviceDropModel>>>>(
                "GetDevice",
                ResponseGet);
        }
        private void ResponseGet(Action<Func<List<DeviceDropModel>>> data)
        {
            data.Invoke(FuncMethod);
        }
        private List<DeviceDropModel> FuncMethod()
        {
            return new List<DeviceDropModel>(
                    this.DeviceList
                        .Where(d => !new string[] { "HL", "VL", "WidthRule", "HeightRule" }.Contains(d.DeviceType) && d.PropList.Count > 0)
                        .Select(d => new DeviceDropModel
                        {
                            DNum = d.DeviceNum,
                            DHeader = d.Header
                        }));
        }




        private void ComponentsInit()
        {
            var ds = _localDataAccess.GetDevices().Select(d =>
                {
                    var dim = new DeviceItemModel(_localDataAccess)
                    {
                        IsMonitor = false,
                        Header = d.Header,
                        X = double.Parse(d.X),
                        Y = double.Parse(d.Y),
                        Z = int.Parse(d.Z),
                        Width = double.Parse(d.W),
                        Height = double.Parse(d.H),
                        DeviceType = d.DeviceTypeName,
                        DeviceNum = d.DeviceNum,

                        FlowDirection = int.Parse(d.FlowDirection),
                        Rotate = int.Parse(d.Rotate),

                        PropList = new ObservableCollection<DevicePropModel>(
                            d.Props.Select(dp => new DevicePropModel
                            {
                                PropName = dp.PropName,
                                PropValue = dp.PropValue,
                            })),

                        VariableList = new ObservableCollection<VariableModel>(
                            d.Vars.Select(dv => new VariableModel
                            {
                                VarNum = dv.VarNum,
                                VarName = dv.Header,
                                VarAddress = dv.Address,
                                Offset = dv.Offset,
                                Modulus = dv.Modulus,
                                VarType = dv.VarType,
                                AlarmConditions = new ObservableCollection<ConditionModel>(dv.AlarmConditions.Select(dvc => new ConditionModel
                                {
                                    CNum = dvc.CNum,
                                    Operator = dvc.Operator,
                                    CompareValue = dvc.CompareValue,
                                    AlarmContent = dvc.AlarmContent,
                                })),
                                UnionConditions = new ObservableCollection<ConditionModel>(dv.UnionConditions.Select(du => new ConditionModel
                                {
                                    CNum = du.CNum,
                                    Operator = du.Operator,
                                    CompareValue = du.CompareValue,

                                    UnionDeviceList = new ObservableCollection<UnionDeviceModel>(du.UnionDevices.Select(dd => new UnionDeviceModel
                                    {
                                        UNum = dd.UNum,
                                        DNum = dd.DNum,
                                        Address = dd.VAddr,
                                        Value = dd.Value,
                                        ValueType = dd.VType
                                    }))
                                })),
                            })),
                        ManualControlList = new ObservableCollection<ManualControlModel>(
                            d.ManualControls.Select(dm => new ManualControlModel
                            {
                                ControlHeader = dm.Header,
                                ControlAddress = dm.Address,
                                Value = dm.Value,
                            })),

                        DeleteCommand = new RelayCommand<DeviceItemModel>(model => DeviceList.Remove(model)),
                        Devices = () => DeviceList.ToList()
                    };
                    // 初始化每个组件的右键菜单 
                    dim.InitContextMenu();

                    return dim;
                });
            DeviceList = new ObservableCollection<DeviceItemModel>(ds);

            // 水平/垂直对齐线
            DeviceList.Add(new DeviceItemModel(_localDataAccess) { X = 0, Y = 0, Width = 2000, Height = 0.5, Z = 9999, DeviceType = "HL", IsVisible = false });
            DeviceList.Add(new DeviceItemModel(_localDataAccess) { X = 0, Y = 0, Width = 0.5, Height = 2000, Z = 9999, DeviceType = "VL", IsVisible = false });
            // 宽度/高度对齐线
            DeviceList.Add(new DeviceItemModel(_localDataAccess) { X = 0, Y = 0, Width = 0, Height = 15, Z = 9999, DeviceType = "WidthRule", IsVisible = false });
            DeviceList.Add(new DeviceItemModel(_localDataAccess) { X = 0, Y = 0, Width = 15, Height = 0, Z = 9999, DeviceType = "HeightRule", IsVisible = false });
        }

        private void DoItemDropCommand(object obj)
        {
            DragEventArgs e = obj as DragEventArgs;
            var data = (ThumbItemModel)e.Data.GetData(typeof(ThumbItemModel));

            var point = e.GetPosition((IInputElement)e.Source);
            var dim = new DeviceItemModel(_localDataAccess)
            {
                Header = data.Header,
                DeviceNum = "D" + DateTime.Now.ToString("yyyyMMddHHmmssFFF"),
                DeviceType = data.TargetType,
                Width = data.Width,
                Height = data.Height,
                X = point.X - data.Width / 2,
                Y = point.Y - data.Height / 2,

                DeleteCommand = new RelayCommand<DeviceItemModel>(model => DeviceList.Remove(model)),
                Devices = () => DeviceList.ToList()
            };
            dim.InitContextMenu();
            DeviceList.Add(dim);
        }

        private void DoSaveCommand(object obj)
        {
            VisualStateManager.GoToElementState(obj as Window, "NormalSuccess", true);
            VisualStateManager.GoToElementState(obj as Window, "SaveFailedNormal", true);

            // 注意一个问题：对齐对象
            var ds = DeviceList
                .Where(d => !new string[] { "HL", "VL", "WidthRule", "HeightRule" }.Contains(d.DeviceType))
                .Select(dev => new DeviceEntity
                {
                    DeviceNum = dev.DeviceNum,
                    X = dev.X.ToString(),
                    Y = dev.Y.ToString(),
                    Z = dev.Z.ToString(),
                    W = dev.Width.ToString(),
                    H = dev.Height.ToString(),
                    DeviceTypeName = dev.DeviceType,
                    Header = dev.Header,

                    FlowDirection = dev.FlowDirection.ToString(),
                    Rotate = dev.Rotate.ToString(),

                    // 转换属性集合
                    Props = dev.PropList.Select(dp => new DevicePropItemEntity
                    {
                        PropName = dp.PropName,
                        PropValue = dp.PropValue,
                    }).ToList(),

                    // 转换点位集合
                    Vars = dev.VariableList.Select(dv => new VariableEntity
                    {
                        VarNum = dv.VarNum,
                        Header = dv.VarName,
                        Address = dv.VarAddress,
                        Offset = dv.Offset,
                        Modulus = dv.Modulus,
                        VarType = dv.VarType,
                        AlarmConditions = dv.AlarmConditions.Select(dva => new ConditionEntity
                        {
                            CNum = dva.CNum,
                            Operator = dva.Operator,
                            CompareValue = dva.CompareValue,
                            AlarmContent = dva.AlarmContent
                        }).ToList(),
                        UnionConditions = dv.UnionConditions.Select(dva => new ConditionEntity
                        {
                            CNum = dva.CNum,
                            Operator = dva.Operator,
                            CompareValue = dva.CompareValue,
                            AlarmContent = dva.AlarmContent,
                            UnionDevices = dva.UnionDeviceList.Select(ud => new UDevuceEntity
                            {
                                UNum = ud.UNum,
                                DNum = ud.DNum,
                                VAddr = ud.Address,
                                Value = ud.Value,
                                VType = ud.ValueType
                            }).ToList()
                        }).ToList()
                    }).ToList(),

                    // 手动控制列表
                    ManualControls = dev.ManualControlList.Select(dm => new ManualEntity
                    {
                        Header = dm.ControlHeader,
                        Address = dm.ControlAddress,
                        Value = dm.Value
                    }).ToList()
                });
            try
            {
                //throw new Exception("保存异常测试，没有执行实际保存逻辑，只用作查看异常提示效果！");

                _localDataAccess.SaveDevice(ds.ToList());

                _windowState = true;
                // 提示保存成功
                VisualStateManager.GoToElementState(obj as Window, "SaveSuccess", true);
            }
            catch (Exception ex)
            {
                SaveFailedMessage = ex.Message;
                // 提示保存失败，包括异常信息
                VisualStateManager.GoToElementState(obj as Window, "SaveFailedShow", true);
            }
        }

        private List<string> InitOptions(string propName, out int OptionsDefaultIndex)
        {
            List<string> values = new List<string>();
            OptionsDefaultIndex = 0;
            switch (propName)
            {
                case "Protocol":
                    values.Add("ModbusRTU");
                    values.Add("ModbusASCII");
                    values.Add("ModbusTCP");
                    values.Add("S7COMM");
                    values.Add("FINSTCP");
                    values.Add("MC3E");
                    break;
                case "PortName":
                    values = new List<string>(SerialPort.GetPortNames());
                    break;
                case "BaudRate":
                    values.Add("2400");
                    values.Add("4800");
                    values.Add("9600");
                    values.Add("19200");
                    values.Add("38400");
                    values.Add("57600");
                    values.Add("115200");

                    OptionsDefaultIndex = 2;
                    break;
                case "DataBit":
                    values.Add("5");
                    values.Add("7");
                    values.Add("8");

                    OptionsDefaultIndex = 2;
                    break;
                case "Parity":
                    values = new List<string>(Enum.GetNames<Parity>());
                    break;
                case "StopBit":
                    values = new List<string>(Enum.GetNames<StopBits>());

                    OptionsDefaultIndex = 1;
                    break;
                case "Endian":
                    values = new List<string>(Enum.GetNames<EndianType>());
                    break;
                default: break;
            }

            return values;
        }
    }
}
