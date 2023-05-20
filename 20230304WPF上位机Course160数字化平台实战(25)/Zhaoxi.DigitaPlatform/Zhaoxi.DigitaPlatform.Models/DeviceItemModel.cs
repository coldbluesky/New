using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using Zhaoxi.DigitaPlatform.IDataAccess;
using Zhaoxi.DigitaPlatform.DeviceAccess;
using Zhaoxi.DigitaPlatform.Entities;
using System.Reflection.Emit;

namespace Zhaoxi.DigitaPlatform.Models
{
    public class DeviceItemModel : ObservableObject
    {
        public Func<List<DeviceItemModel>> Devices { get; set; }

        public string DeviceNum { get; set; }
        public string Header { get; set; }

        private bool _isSelected;

        int z_temp = 0;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                Set(ref _isSelected, value);

                if (value)
                {
                    z_temp = this.Z;
                    this.Z = 999;
                }
                else this.Z = z_temp;
            }
        }

        private bool _isVisible = true;

        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(ref _isVisible, value); }
        }


        private double x;
        public double X
        {
            get { return x; }
            set { Set(ref x, value); }
        }
        private double y;
        public double Y
        {
            get { return y; }
            set { Set(ref y, value); }
        }
        private int z = 0;

        public int Z
        {
            get { return z; }
            set { Set(ref z, value); }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set { Set(ref _width, value); }
        }
        private double _height;
        public double Height
        {
            get { return _height; }
            set { Set(ref _height, value); }
        }

        private int _rotate;

        public int Rotate
        {
            get { return _rotate; }
            set { Set(ref _rotate, value); }
        }

        private int _flowDirection;

        public int FlowDirection
        {
            get { return _flowDirection; }
            set { Set(ref _flowDirection, value); }
        }

        private bool _isWarning;

        public bool IsWarning
        {
            get { return _isWarning; }
            set { Set(ref _isWarning, value); }
        }
        private DeviceAlarmModel _warningMessage;

        public DeviceAlarmModel WarningMessage
        {
            get { return _warningMessage; }
            set { Set(ref _warningMessage, value); }
        }

        private bool _isMonitor = true;
        /// <summary>
        /// 是否是监控状态
        /// </summary>
        public bool IsMonitor
        {
            get { return _isMonitor; }
            set { Set(ref _isMonitor, value); }
        }



        // 根据这个名称动态创建一个组件实例
        public string DeviceType { get; set; }


        // 子项删除命令
        public RelayCommand<DeviceItemModel> DeleteCommand { get; set; }

        public RelayCommand AddPropCommand { get; set; }
        public RelayCommand<DevicePropModel> DeletePropCommand { get; set; }

        public RelayCommand AddVariableCommand { get; set; }
        public RelayCommand<VariableModel> DeleteVariableCommand { get; set; }

        public RelayCommand AddManualControlCommand { get; set; }
        public RelayCommand<ManualControlModel> DeleteManualControlCommand { get; set; }


        // 手动控制命令
        public RelayCommand<ManualControlModel> ManualControlCommand { get; set; }

        #region 缩放动作命令
        public RelayCommand<object> ResizeDownCommand { get; set; }
        public RelayCommand<object> ResizeMoveCommand { get; set; }
        public RelayCommand<object> ResizeUpCommand { get; set; }
        #endregion




        public ObservableCollection<DevicePropModel> PropList { get; set; } = new ObservableCollection<DevicePropModel>();

        public ObservableCollection<VariableModel> VariableList { get; set; } = new ObservableCollection<VariableModel>();

        public ObservableCollection<ManualControlModel> ManualControlList { get; set; } =
            new ObservableCollection<ManualControlModel>()
            {
                //new ManualControlModel{ ControlHeader="远程启动",ControlAddress="40008",Value="1"},
                //new ManualControlModel{ ControlHeader="远程停机",ControlAddress="40008",Value="2"},
                //new ManualControlModel{ ControlHeader="卸载",ControlAddress="40008",Value="4"},
                //new ManualControlModel{ ControlHeader="加载",ControlAddress="40008",Value="8"},
            };


        ILocalDataAccess _localDataAccess;
        public DeviceItemModel(ILocalDataAccess localDataAccess)
        {
            _localDataAccess = localDataAccess;

            AddPropCommand = new RelayCommand(() =>
            {
                PropList.Add(new DevicePropModel() { PropName = "Protocol", PropValue = "ModbusRtu" });
            });

            DeletePropCommand = new RelayCommand<DevicePropModel>(model => PropList.Remove(model));


            AddVariableCommand = new RelayCommand(() =>
            {
                VariableList.Add(new VariableModel() { VarNum = "V" + DateTime.Now.ToString("yyyyMMddHHmmssFFF") });
            });
            DeleteVariableCommand = new RelayCommand<VariableModel>(model => VariableList.Remove(model));


            AddManualControlCommand = new RelayCommand(() =>
            {
                ManualControlList.Add(new ManualControlModel());
            });
            DeleteManualControlCommand = new RelayCommand<ManualControlModel>(model =>
            {
                ManualControlList.Remove(model);
            });

            ManualControlCommand = new RelayCommand<ManualControlModel>(model =>
            {
                // 设备的通信参数
                // 手动写入地址
                // 通信对象 Communication

                Communication communication = Communication.Create();
                // 准备通信对象
                var re = communication.GetExecuteObject(this.PropList.Select(p =>
                    new DevicePropItemEntity { PropName = p.PropName, PropValue = p.PropValue }
                    ).ToList());
                if (!re.Status)
                {
                    this.IsWarning = true;
                    this.WarningMessage = new DeviceAlarmModel
                    {
                        AlarmContent = re.Message
                    };
                    return;
                }

                // 解析地址，不需要打包，直接解析 
                var ra = re.Data.AnalysisAddress(
                    new DeviceAccess.Base.VariableProperty
                    {
                        VarAddr = model.ControlAddress,
                        ValueType = typeof(UInt16)
                    }, true);
                if (!ra.Status)
                {
                    this.IsWarning = true;
                    this.WarningMessage = new DeviceAlarmModel
                    {
                        AlarmContent = ra.Message
                    };
                    return;
                }
                ra.Data.ValueBytes = BitConverter.GetBytes(UInt16.Parse(model.Value)).Reverse().ToArray();
                var rw = re.Data.Write(new List<DeviceAccess.Base.CommAddress> { ra.Data });
                if (!rw.Status)
                {
                    this.IsWarning = true;
                    this.WarningMessage = new DeviceAlarmModel
                    {
                        AlarmContent = rw.Message
                    };
                    return;
                }
            });

            // 处理组件尺寸缩放
            ResizeDownCommand = new RelayCommand<object>(DoResizeDown);
            ResizeMoveCommand = new RelayCommand<object>(DoResizeMove);
            ResizeUpCommand = new RelayCommand<object>(DoResizeUp);

        }

        Point startP = new Point(0, 0);
        #region 处理组件移动逻辑
        bool isMoving = false;

        List<DeviceItemModel> devicesTemp = new List<DeviceItemModel>();
        List<DeviceItemModel> lineTemp = new List<DeviceItemModel>();

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取光标对于拖动对象的坐标值
            startP = e.GetPosition((System.Windows.IInputElement)sender);
            isMoving = true;

            // 获取所有设备的列表
            devicesTemp = Devices().Where(d => !new string[] { "HL", "VL", "WidthRule", "HeightRule" }.Contains(d.DeviceType) && d != this).ToList();
            lineTemp = Devices().Where(d => new string[] { "HL", "VL" }.Contains(d.DeviceType)).ToList();

            // 鼠标光标捕获
            Mouse.Capture((IInputElement)sender);

            e.Handled = true;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            // 移动 是在按下 之后
            if (isMoving)
            {
                // 计算出新的位置
                // 相对的是Canvas画布
                // 可以通过视觉树查找  
                // 这个坐标应该是拖动对象的新位置 
                Point p = e.GetPosition(GetParent((FrameworkElement)sender));

                double _x = p.X - startP.X;
                double _y = p.Y - startP.Y;
                // 处理X方向的对齐（纵线）
                {
                    // 计算的甩组件与当前组件之间的X距离
                    double minDistance = devicesTemp.Min(d => Math.Abs(d.X - _x));
                    var line = lineTemp.First(l => l.DeviceType == "VL");

                    if (minDistance < 20)
                    {
                        // 距离在20以内，显示对齐线
                        var dim = devicesTemp.FirstOrDefault(d => Math.Abs(Math.Abs(d.X - _x) - minDistance) < 1);
                        if (dim != null)
                        {
                            line.IsVisible = true;
                            line.X = dim.X;

                            if (minDistance < 10)
                                _x = dim.X;
                        }
                    }
                    else
                        line.IsVisible = false;
                }

                // 处理Y方向的对齐（横线）
                {
                    double minDistance = devicesTemp.Min(d => Math.Abs(d.Y - _y));
                    var line = lineTemp.First(l => l.DeviceType == "HL");
                    if (minDistance < 20)
                    {
                        // 距离在20以内，显示对齐线
                        var dim = devicesTemp.FirstOrDefault(d => Math.Abs(Math.Abs(d.Y - _y) - minDistance) < 1);
                        if (dim != null)
                        {
                            line.IsVisible = true;
                            line.Y = dim.Y;

                            if (minDistance < 10)
                                _y = dim.Y;
                        }
                    }
                    else
                        line.IsVisible = false;
                }

                // 数据驱动   通过数据模型中的属性变化 ，驱使页面对象的呈现改变
                X = _x;
                Y = _y;
            }
        }

        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            foreach (var item in lineTemp)
            {
                item.IsVisible = false;
            }
            // 释放光标
            Mouse.Capture(null);
        }

        private Canvas GetParent(FrameworkElement d)
        {
            dynamic obj = VisualTreeHelper.GetParent(d);
            if (obj != null && obj is Canvas)
                return obj;

            return GetParent(obj);
        }
        #endregion

        #region 处理组件尺寸缩放逻辑
        bool _isResize = false;
        double oldWidth, oldHeight;

        DeviceItemModel WR;
        DeviceItemModel HR;

        private void DoResizeDown(object obj)
        {
            var e = obj as MouseButtonEventArgs;
            _isResize = true;
            startP = e.GetPosition(GetParent((FrameworkElement)e.Source));

            // 获取所有可做比对的其他组件
            devicesTemp = Devices().Where(d => !new string[] { "HL", "VL", "WidthRule", "HeightRule" }.Contains(d.DeviceType) && d != this).ToList();

            oldWidth = this.Width;
            oldHeight = this.Height;

            // 获取相对Canvas的按下坐标
            Mouse.Capture((IInputElement)e.Source);
            e.Handled = true;
        }
        private void DoResizeMove(object obj)
        {
            var e = obj as MouseEventArgs;
            ReleaseRule();
            if (_isResize)
            {
                // 鼠标光标的新位置
                Point current = e.GetPosition(GetParent((FrameworkElement)e.Source));
                // 根据光标类型判断是如何变化 
                var c = (e.Source as Ellipse).Cursor;
                if (c != null)
                {
                    if (c == Cursors.SizeWE)// 水平方向
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Alt)  // 移动过程中检查Alt按下，不做对齐
                            this.Width = oldWidth + current.X - startP.X;
                        else
                            this.Width = ShowWidthRule(oldWidth + current.X - startP.X);
                    }
                    else if (c == Cursors.SizeNS)// 垂直方向
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Alt)
                            this.Height = oldHeight + current.Y - startP.Y;
                        else
                            this.Height = ShowHeightRule(oldHeight + current.Y - startP.Y);
                    }
                    else if (c == Cursors.SizeNWSE)// 右下方向
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            // 锁定比例
                            var rate = this.Width / this.Height;
                            this.Width = ShowWidthRule(oldWidth + current.X - startP.X);
                            this.Height = this.Width / rate;
                        }
                        else
                        {
                            this.Width = ShowWidthRule(oldWidth + current.X - startP.X);
                            this.Height = ShowHeightRule(oldHeight + current.Y - startP.Y);
                        }
                    }
                }
                e.Handled = true;
            }
        }
        private void DoResizeUp(object obj)
        {
            _isResize = false;
            var e = obj as MouseButtonEventArgs;
            e.Handled = true;
            Mouse.Capture(null);
            ReleaseRule();
        }

        private void ReleaseRule()
        {
            if (WR != null)
            {
                WR.IsVisible = false;
                WR = null;
            }
            if (HR != null)
            {
                HR.IsVisible = false;
                HR = null;
            }
        }

        private double ShowWidthRule(double width)
        {
            if (WR == null)
                WR = Devices().FirstOrDefault(d => d.DeviceType == "WidthRule");
            var dim = devicesTemp.FirstOrDefault(d => Math.Abs(d.Width - width) < 20);
            if (dim != null)
            {
                WR.IsVisible = true;
                WR.Width = dim.Width;
                WR.Y = dim.Y + dim.Height + 5;
                WR.X = dim.X;

                if (Math.Abs(dim.Width - width) < 5)
                    width = dim.Width;
            }
            else WR.IsVisible = false;

            return width;
        }

        private double ShowHeightRule(double heigth)
        {
            if (HR == null)
                HR = Devices().FirstOrDefault(d => d.DeviceType == "HeightRule");
            var dim = devicesTemp.FirstOrDefault(d => Math.Abs(d.Height - heigth) < 20);
            if (dim != null)
            {
                HR.IsVisible = true;
                HR.Height = dim.Height;
                HR.Y = dim.Y;
                HR.X = dim.X + dim.Width + 5;

                if (dim.Height - heigth < 5)
                    heigth = dim.Height;
            }
            else HR.IsVisible = false;

            return heigth;
        }
        #endregion

        #region 初始化右键菜单 
        public List<Control> ContextMenus { get; set; }
        public void InitContextMenu()
        {
            ContextMenus = new List<Control>();
            ContextMenus.Add(new MenuItem
            {
                Header = "顺时针旋转",
                Command = new RelayCommand(() => this.Rotate += 90),
                Visibility = new string[] {
                    "RAJoints", "TeeJoints","Temperature","Humidity","Pressure","Flow","Speed"
                }.Contains(this.DeviceType) ? Visibility.Visible : Visibility.Collapsed
            });
            ContextMenus.Add(new MenuItem
            {
                Header = "逆时针旋转",
                Command = new RelayCommand(() => this.Rotate -= 90),
                Visibility = new string[] {
                    "RAJoints", "TeeJoints","Temperature","Humidity","Pressure","Flow","Speed"
                }.Contains(this.DeviceType) ? Visibility.Visible : Visibility.Collapsed
            });
            ContextMenus.Add(new MenuItem
            {
                Header = "改变流向",
                Command = new RelayCommand(() => this.FlowDirection = (++this.FlowDirection) % 2),
                Visibility = new string[] { "HorizontalPipeline", "VerticalPipeline" }.Contains(this.DeviceType) ? Visibility.Visible : Visibility.Collapsed
            });

            ContextMenus.Add(new Separator());

            ContextMenus.Add(new MenuItem
            {
                Header = "向上一层",
                Command = new RelayCommand(() => this.Z++)
            });
            ContextMenus.Add(new MenuItem
            {
                Header = "向下一层",
                Command = new RelayCommand(() => this.Z--)
            });
            ContextMenus.Add(new Separator { });

            ContextMenus.Add(new MenuItem
            {
                Header = "删除",
                Command = this.DeleteCommand,
                CommandParameter = this
            });

            var cms = ContextMenus.Where(cm => cm.Visibility == Visibility.Visible).ToList();
            foreach (var item in cms)
            {
                if (item is Separator)
                    item.Visibility = Visibility.Collapsed;
                else break;
            }

        }
        #endregion
    }
}
