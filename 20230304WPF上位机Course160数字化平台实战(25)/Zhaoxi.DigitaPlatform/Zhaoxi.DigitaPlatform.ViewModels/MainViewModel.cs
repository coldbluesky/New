using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Shapes;
using Zhaoxi.DigitaPlatform.Common;
using Zhaoxi.DigitaPlatform.DeviceAccess;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.Entities;
using Zhaoxi.DigitaPlatform.IDataAccess;
using Zhaoxi.DigitaPlatform.Models;

namespace Zhaoxi.DigitaPlatform.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _systemTitle;

        public string SystemTile
        {
            get { return _systemTitle; }
            set { Set(ref _systemTitle, value); }
        }

        private int _viewBlur = 0;

        public int ViewBlur
        {
            get { return _viewBlur; }
            set { Set(ref _viewBlur, value); }
        }

        public UserModel GlobalUserInfo { get; set; } = new UserModel();

        private object _viewContent;

        public object ViewContent
        {
            get { return _viewContent; }
            set { Set(ref _viewContent, value); }
        }

        private bool _isWindowClose;

        public bool IsWindowClose
        {
            get { return _isWindowClose; }
            set { Set(ref _isWindowClose, value); }
        }



        public List<MenuModel> Menus { get; set; }
        public RelayCommand<object> SwitchPageCommand { get; set; }


        // Monitor
        public VariableModel Temperature { get; set; }
        public VariableModel Humidity { get; set; }
        public VariableModel PM { get; set; }
        public VariableModel Pressure { get; set; }
        public VariableModel FlowRate { get; set; }

        public List<RankingItemModel> RankingList { get; set; }
        public List<MonitorWarnningModel> WarnningList { get; set; }

        public List<DeviceItemModel> DeviceList { get; set; }

        public RelayCommand ComponentsConfigCommand { get; set; }

        public RelayCommand LogoutCommand { get; set; }

        // 组件的详情查看命令
        public RelayCommand AlarmDetailCommand { get; set; }

        ILocalDataAccess _localDataAccess;
        public MainViewModel(ILocalDataAccess localDataAccess)
        {
            _localDataAccess = localDataAccess;
            if (!IsInDesignMode)
            {
                // 主窗口数据
                #region 主窗口菜单 
                Menus = new List<MenuModel>();
                Menus.Add(new MenuModel
                {
                    IsSelected = true,
                    MenuHeader = "监控",
                    MenuIcon = "\ue639",
                    TargetView = "MonitorPage"
                });
                Menus.Add(new MenuModel
                {
                    MenuHeader = "趋势",
                    MenuIcon = "\ue61a",
                    TargetView = "TrendPage"
                });
                Menus.Add(new MenuModel
                {
                    MenuHeader = "报警",
                    MenuIcon = "\ue60b",
                    TargetView = "AlarmPage"
                });
                Menus.Add(new MenuModel
                {
                    MenuHeader = "报表",
                    MenuIcon = "\ue703",
                    TargetView = "ReportPage"
                });
                Menus.Add(new MenuModel
                {
                    MenuHeader = "配置",
                    MenuIcon = "\ue60f",
                    TargetView = "SettingsPage"
                });
                #endregion

                ShowPage(Menus[0]);
                SwitchPageCommand = new RelayCommand<object>(ShowPage);

                // Monitor
                Random random = new Random();
                #region 用气排行
                string[] quality = new string[] { "车间-1", "车间-2", "车间-3", "车间-4",
                "车间-5" };
                RankingList = new List<RankingItemModel>();
                foreach (var q in quality)
                {
                    RankingList.Add(new RankingItemModel()
                    {
                        Header = q,
                        PlanValue = random.Next(100, 200),
                        FinishedValue = random.Next(10, 150),
                    });
                }
                #endregion

                #region 设备提醒
                WarnningList = new List<MonitorWarnningModel>()
                {
                  new MonitorWarnningModel{Message= "朝夕PLT-01：保养到期",
                      DateTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                  new MonitorWarnningModel{Message= "朝夕PLT-01：故障",
                      DateTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                  new MonitorWarnningModel{Message= "朝夕PLT-01：保养到期",
                      DateTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                  new MonitorWarnningModel{Message= "朝夕PLT-01：保养到期",
                      DateTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                  new MonitorWarnningModel{Message= "朝夕PLT-01：保养到期",
                      DateTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                  new MonitorWarnningModel{Message= "朝夕PLT-01：保养到期",
                      DateTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                  new MonitorWarnningModel{Message= "朝夕PLT-01：保养到期",
                      DateTime=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                };
                #endregion
                ComponentsConfigCommand = new RelayCommand(ShowConfig);

                // 初始化组件信息
                this.ComponentsInit();

                //DeviceList[0].IsWarning = true;// 测试 查看效果
                //DeviceList[0].WarningMessage = "设备异常信息测试；设备异常信息测试；设备异常信息测试；设备异常信息测试；";

                // 初始化固定监测参数
                List<BaseInfoEntity> baseInfos = new List<BaseInfoEntity>();
                localDataAccess.GetBaseInfo(baseInfos, null);
                SystemTile = baseInfos.FirstOrDefault(b => b.BaseNum == "B001").Value;

                //var vars = baseInfos.Where(b => b.type == "2").Select(b => new { b.BaseNum, b.DeviceNum, b.VariableNum }).ToList();
                var var = baseInfos.FirstOrDefault(v => v.BaseNum == "B005");
                // 注意检查空对象
                // 温度的对象指定
                Temperature = DeviceList.FirstOrDefault(d => d.DeviceNum == var.DeviceNum)
                    .VariableList.FirstOrDefault(v => v.VarNum == var.VariableNum);

                // 开始获取设备数据
                // 两个方法任选其一
                // 本地直接监听设备数据
                this.Monitor();

                // 通过服务监听设备数据
                //this.MonitorByServer();

                // 连接到MQTT服务器，准备数据云端对接
                CloudService();


                LogoutCommand = new RelayCommand(DoLogout);

                AlarmDetailCommand = new RelayCommand(() =>
                {
                    Menus[2].IsSelected = true;
                    ShowPage(Menus[2]);
                });
            }

            //Task.Run(() =>
            //{
            //    // while()
            //    // 从数据队列中获取需要上传的数据
            //});


            //Messenger.Default.Register<DeviceAlarmModel>(this, "Alarm", ReceiveAlarm);

            Messenger.Default.Register<Action<Func<string[], List<DeviceItemModel>>>>(this, "DeviceInfo",
                   new Action<Action<Func<string[], List<DeviceItemModel>>>>(a => a.Invoke(ds =>
                   {
                       return this.DeviceList.Where(d => ds.Any(dd => dd == d.DeviceNum)).ToList();
                   })));
            Messenger.Default.Register<string>(this, "CancelAlarm", num =>
                {
                    var device = DeviceList.FirstOrDefault(d => d.DeviceNum == num);
                    if (device == null) return;
                    device.IsWarning = false;
                });
        }

        private void ReceiveAlarm(DeviceAlarmModel model)
        {
            var device = DeviceList.FirstOrDefault(d => d.DeviceNum == model.DNum);
            if (device != null && !device.IsWarning)
            {
                device.WarningMessage = model;
                this.SetWarning(model.ANum, model.AlarmContent, device, model.AlarmValue, model.VNum);
            }
        }

        CancellationTokenSource cts = new CancellationTokenSource();
        List<Task> tasks = new List<Task>();
        Communication communication = Communication.Create();
        private void Monitor()
        {
            // 通过统一的接口获取通信对象（提供对应的属性）
            // 获取对象后，进行方法的调用 
            // 多设备请求：
            // 1、设备集合依次轮询(一个设备处理完后，关闭连接，再开下个连接--短连接)  
            // 2、每个设备单独监视（每个设备一个线程，一个连接打开的后，直到应用支出再关闭连接）
            //    场景：设备   分离，   从一个设备中转     不允许多个连接    ModbusRTU   串口总线
            //    需要解决传输对象共用的问题    --- 之后多线程同时处理   对象 上锁
            foreach (var item in DeviceList)
            {
                // 如果没有配置相关数据，不执行监视
                if (item.PropList.Count == 0 || item.VariableList.Count == 0) continue;

                // 根据配置信息，创建协议执行对象
                var result_eo = communication.GetExecuteObject(
                    item.PropList.Select(p =>
                    new DevicePropItemEntity { PropName = p.PropName, PropValue = p.PropValue }
                    ).ToList());
                if (!result_eo.Status)
                {
                    // 异常提示信息
                    //item.IsWarning = true;// 设备报警状态
                    //item.WarningMessage = result_eo.Message;
                    SetWarning("BE0001", result_eo.Message, item, vnum: "01");
                    continue;
                }

                // 根据协议对象，创建循环监视线程
                var task = Task.Run(async () =>
                {
                    // 处理刷新频率
                    int delay = 500;
                    var dv = item.PropList.FirstOrDefault(p => p.PropName == "RefreshRate");
                    if (dv != null)
                        int.TryParse(dv.PropValue, out delay);

                    /// 将当前设备中的所有点位数据   提供到Read方法
                    /// ushort
                    /// 
                    //Type.GetType("System.UInt16");
                    List<VariableProperty> variables =
                    item.VariableList.Select(v => new VariableProperty
                    {
                        VarId = v.VarNum,
                        VarAddr = v.VarAddress,
                        ValueType = Type.GetType("System." + v.VarType)
                    }).ToList();

                    Result<List<CommAddress>> result_addr = result_eo.Data.GroupAddress(variables);
                    if (!result_addr.Status)
                    {
                        // 异常提示信息
                        //item.IsWarning = true;
                        //item.WarningMessage = result_addr.Message;
                        SetWarning("BE0002", result_addr.Message, item, vnum: "02");
                        return;
                    }

                    // 计算对象
                    DataTable dataTable = new DataTable();
                    // 循环逻辑
                    while (!cts.IsCancellationRequested)
                    {
                        await Task.Delay(delay);
                        /// 处理数据 

                        var result_value = result_eo.Data.Read(result_addr.Data);  // 目前：里面做打包   
                        if (!result_value.Status)
                        {
                            // 异常提示信息
                            //item.IsWarning = true;
                            //item.WarningMessage = result_value.Message;
                            SetWarning("BE0003", result_addr.Message, item, vnum: "03");
                            continue;
                        }
                        // 解析数据
                        foreach (var ma in result_addr.Data)
                        {
                            foreach (var vv in ma.Variables)
                            {
                                var dataBytes = vv.ValueBytes;
                                var id = vv.VariableId;
                                var dVar = item.VariableList.FirstOrDefault(v => v.VarNum == id);// 设备的变量记录
                                                                                                 //vp.VarId
                                                                                                 //vp.ValueBytes

                                // 需要进一步处理
                                //dVar.Value = communication.ConvertType(dataBytes, Type.GetType("System." + dVar.VarType));
                                Result<object> result_data = communication.ConvertType(dataBytes, Type.GetType("System." + dVar.VarType));
                                if (!result_data.Status)
                                {
                                    //item.IsWarning = true;
                                    //item.WarningMessage = result_data.Message;
                                    SetWarning("BE0004", result_data.Message, item, vnum: dVar.VarNum);
                                    continue;
                                }
                                try
                                {
                                    // 获取的设备数据是200（result_data.Data）
                                    // 偏移量(dVar.Offset)  -20
                                    // 系数(dVar.Modulus)   0.1  
                                    // "200 * 0.1 + -20"  
                                    // 0
                                    var newValue = result_data.Data;
                                    if (dVar.VarType != "Boolean")
                                        newValue = new DataTable().Compute(result_data.Data.ToString() + "*" + dVar.Modulus.ToString() + "+" + dVar.Offset.ToString(), "");

                                    if (dVar.Value.Equals(newValue)) continue;
                                    dVar.Value = newValue;

                                    string alarm_num = "";
                                    string union_num = "";

                                    //// 报警条件检查
                                    if (dVar.AlarmConditions.Count > 0)
                                    {
                                        ConditionModel cm = null;
                                        foreach (var dc in dVar.AlarmConditions)
                                        {
                                            string condition = dVar.Value + dc.Operator + dc.CompareValue;
                                            if (Boolean.TryParse(new DataTable().Compute(condition, "").ToString(), out bool result) && result)
                                            {
                                                if (!new string[] { "==", "!=" }.Contains(condition))
                                                {
                                                    cm = this.CompareValues(dVar.AlarmConditions.ToList(), dc.Operator, dVar.Value);
                                                }
                                            }
                                        }
                                        if (cm != null)
                                        {
                                            alarm_num = cm.CNum;

                                            DeviceAlarmModel model = new DeviceAlarmModel
                                            {
                                                ANum = "A-" + DateTime.Now.ToString("yyyyMMddHHmmssFFF"),
                                                CNum = cm.CNum,
                                                DNum = dVar.DeviceNum,
                                                VNum = dVar.VarNum,
                                                AlarmContent = cm.AlarmContent,
                                                AlarmValue = dVar.Value.ToString(),
                                                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                                Level = 1,
                                                State = 0
                                            };
                                            //Messenger.Default.Send<DeviceAlarmModel>(model, "Alarm");

                                            ReceiveAlarm(model);
                                        }
                                    }
                                    // 自动联控条件逻辑处理
                                    if (dVar.UnionConditions.Count > 0)
                                    {
                                        foreach (var dc in dVar.AlarmConditions)
                                        {
                                            string condition = dVar.Value + dc.Operator + dc.CompareValue;

                                            if (!Boolean.TryParse(new DataTable().Compute(condition, "").ToString(), out bool result) || !result)
                                                continue;

                                            union_num = dc.CNum;

                                            // 触发条件
                                            foreach (var du in dc.UnionDeviceList)
                                            {
                                                var d = DeviceList.FirstOrDefault(dd => dd.DeviceNum == du.DNum);
                                                if (d == null) continue;

                                                // 1、创建通信对象
                                                var re = communication.GetExecuteObject(
                                                                d.PropList.Select(p =>
                                                                    new DevicePropItemEntity { PropName = p.PropName, PropValue = p.PropValue }
                                                                    ).ToList());
                                                if (!re.Status) continue;
                                                // 2、地址解析
                                                var rd = re.Data.AnalysisAddress(new VariableProperty
                                                {
                                                    VarAddr = du.Address,
                                                    ValueType = Type.GetType("System." + du.ValueType)
                                                });
                                                if (!rd.Status) continue;
                                                // 3、数据的byte数组初始化
                                                rd.Data.ValueBytes = BitConverter.GetBytes(UInt16.Parse(du.Value)).Reverse().ToArray();

                                                // 4、利用第1步通信对象进行写入请求，传入第2、3部分的结果信息
                                                var rw = re.Data.Write(new List<CommAddress> { rd.Data });
                                                if (!rw.Status) continue;
                                            }

                                        }
                                    }


                                    // 提交一次，数据性能：  数据缓存列表   100   执行一次提交
                                    ViewModelLocator.AddRecord(new RecordWriteEntity
                                    {
                                        DeviceNum = dVar.DeviceNum,
                                        DeviceName = DeviceList.FirstOrDefault(dd => dd.DeviceNum == dVar.DeviceNum)?.Header,
                                        VarNum = dVar.VarNum,
                                        VarName = dVar.VarName,
                                        RecordValue = dVar.Value.ToString(),
                                        AlarmNum = alarm_num,
                                        UnionNum = union_num,
                                        RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        UserName = GlobalUserInfo.UserName
                                    });


                                    // 数据上云
                                    MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                                        .WithTopic("pub/" + dVar.DeviceNum + "/" + dVar.VarNum)
                                        .WithPayload(Encoding.Default.GetBytes(dVar.Value.ToString()))
                                        .WithRetainFlag(false)
                                        .Build();
                                    await client.PublishAsync(message);
                                }
                                catch (Exception ex)
                                {
                                    //item.IsWarning = true;
                                    //item.WarningMessage = ex.Message;
                                    SetWarning("BE0005", ex.Message, item, vnum: dVar.VarNum);
                                }
                            }
                        }

                        //item.IsWarning = false;
                        // 这个消息什么时候取消？
                    }

                    result_eo.Data.Dispose();

                }, cts.Token);
                tasks.Add(task);
            }
        }

        #region 通过服务进行监听
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private ushort clientId = 0;
        private ushort tid = 0;

        bool alive = false;
        AutoResetEvent PingResetEvent = new AutoResetEvent(false);
        private void MonitorByServer()
        {
            try
            {
                // 1、TCP连接到服务器
                socket.Connect("127.0.0.1", 8899);
                socket.ReceiveTimeout = 5000;

                // 2、获取服务器颁发的ID
                byte[] idbytes = this.ReceiveBytes(socket);
                if (idbytes[4] == 0xFF)
                    return;

                clientId = BitConverter.ToUInt16(new byte[] { idbytes[8], idbytes[7] }, 0);

                // 3、提交订阅信息：所有设备的通信属性、变量信息
                foreach (var item in DeviceList)
                {
                    if (item.PropList.Count == 0 || item.VariableList.Count == 0) continue;

                    // "\"Protocl:ModbusRTU\",\"SlaveId:1\""    可以直接序列化Model  
                    var ps = string.Join(",", item.PropList.Select(p => p.PropName + ":" + p.PropValue).ToArray());
                    byte[] pb = Encoding.Default.GetBytes(ps);
                    var vs = string.Join(",", item.VariableList.Select(v => v.DeviceNum + "-" + v.VarNum + ":" + v.VarAddress + ":" + v.VarType).ToArray());
                    byte[] vb = Encoding.Default.GetBytes(vs);

                    // 订阅信息发送
                    tid++;
                    tid %= 0xFFFF;
                    List<byte> subBytes = new List<byte>
                    {
                        (byte)(tid / 256),
                        (byte)(tid % 256),
                        (byte)(clientId / 256),
                        (byte)(clientId % 256),
                        0x03,
                        (byte)((pb.Length + vb.Length + 4) / 256),
                        (byte)((pb.Length + vb.Length + 4) % 256),
                        (byte)((pb.Length) / 256),
                        (byte)((pb.Length) % 256)
                    };
                    subBytes.AddRange(pb);
                    subBytes.Add((byte)((vb.Length) / 256));
                    subBytes.Add((byte)((vb.Length) % 256));
                    subBytes.AddRange(vb);

                    // 发送订阅请求
                    socket.Send(subBytes.ToArray());

                    // 接收订阅反馈（自行处理）
                    // 
                }

                // 4、开启数据接收
                var t = Task.Factory.StartNew(async () =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        await Task.Delay(100);
                        try
                        {
                            byte[] respBytes = this.ReceiveBytes(socket);

                            switch (respBytes[4])
                            {
                                case 0x04:
                                    {
                                        // 接收到订阅后，服务端所发布的最新数据
                                        var dataBytes = respBytes.ToList().GetRange(7, respBytes.Length - 7);

                                        // VarId
                                        // id
                                        ushort idlen = BitConverter.ToUInt16(new byte[] { dataBytes[1], dataBytes[0] });
                                        var idBytes = dataBytes.GetRange(2, idlen);
                                        string id_str = Encoding.Default.GetString(idBytes.ToArray());
                                        string[] ids = id_str.Split("-");

                                        var d = this.DeviceList.FirstOrDefault(d => d.DeviceNum == ids[0]);
                                        if (d == null) continue;
                                        var v = d.VariableList.FirstOrDefault(v => v.VarNum == ids[1]);
                                        if (v == null) continue;


                                        int i = 2 + idlen;
                                        ushort vlen = BitConverter.ToUInt16(new byte[] { dataBytes[i + 1], dataBytes[i] });
                                        var vBytes = dataBytes.GetRange(i + 2, vlen);

                                        Result<object> result_data = communication.ConvertType(vBytes.ToArray(), Type.GetType("System." + v.VarType));
                                        // 检查结果状态 

                                        // 赋值
                                        var newValue = result_data.Data;
                                        if (v.VarType != "Boolean")
                                            newValue = new DataTable().Compute(result_data.Data.ToString() + "*" + v.Modulus.ToString() + "+" + v.Offset.ToString(), "");

                                        if (v.Value == newValue) continue;
                                        v.Value = newValue;

                                        break;
                                    }
                                case 0x06:
                                    // 处理心跳
                                    alive = true;
                                    PingResetEvent.Set();
                                    break;
                                default: break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }, cts.Token);
                tasks.Add(t);

                // 5、开启心跳
                t = Task.Factory.StartNew(async () =>
                {
                    Socket s = socket;
                    while (!cts.IsCancellationRequested)
                    {
                        // 每隔一秒发一个心跳
                        await Task.Delay(1000);

                        tid++;
                        tid %= 0xFFFF;
                        List<byte> subBytes = new List<byte> {
                                (byte)(tid / 256), (byte)(tid % 256),
                                (byte)(clientId/256),(byte)(clientId%256),
                                0x06 ,
                                0x00,0x00
                            };
                        s.Send(subBytes.ToArray(), 0, subBytes.Count, SocketFlags.None);
                        // 接收心跳响应

                        alive = false;
                        PingResetEvent.WaitOne(3000);
                        // 扩展：尝试多次心跳，都不能正常，判断断开 ，执行重连
                        if (!alive)
                            break;
                    }
                }, cts.Token);
                tasks.Add(t);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private byte[] ReceiveBytes(Socket socket)
        {
            byte[] subResp = new byte[7];
            socket.Receive(subResp, 0, 7, SocketFlags.None);
            ushort len = BitConverter.ToUInt16(new byte[] { subResp[6], subResp[5] });
            if (len == 0)
                return subResp;

            byte[] pyload = new byte[len];
            socket.Receive(pyload, 0, len, SocketFlags.None);

            List<byte> bytes = new List<byte>();
            bytes.AddRange(subResp);
            bytes.AddRange(pyload);

            return bytes.ToArray();
        }
        #endregion

        #region MQTT
        IManagedMqttClient client;
        private void CloudService()
        {
            client = new MqttFactory().CreateManagedMqttClient();
            // 连接到MQTT服务器
            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e =>
            {
                MqttTopicFilter topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("cmd/#")// 主题
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                .Build();
                client.SubscribeAsync(topicFilter);
            });
            //client.DisconnectedHandler
            client.ApplicationMessageReceivedHandler = 
                new MqttApplicationMessageReceivedHandlerDelegate(Client_ApplicationMessageReceived);

            // IP   Port   Client ID
            // UserName  Password
            IMqttClientOptions clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("47.109.25.112", 1883)
                .WithCredentials("admin", "123456")
                .Build();
            IManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(clientOptions)
                .Build();

            client.StartAsync(options);// 异步连接到服务器
        }
        void Client_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine(">>> 收到消息:" + e.ApplicationMessage.ConvertPayloadToString() + ",来自客户端" + e.ClientId + ",主题:" + e.ApplicationMessage.Topic);

            string[] ts = e.ApplicationMessage.Topic.Split('/');
            var dev = DeviceList.FirstOrDefault(d => d.DeviceNum == ts[1]);
            var result_eo = communication.GetExecuteObject(dev.PropList.Select(p =>
               new DevicePropItemEntity { PropName = p.PropName, PropValue = p.PropValue }
               ).ToList());
            if (!result_eo.Status)
            {
                // 异常提示信息
                return;
            }

            var result_addr = result_eo.Data.AnalysisAddress(new VariableProperty
            {
                VarAddr = ts[2],
                ValueType = typeof(UInt16)
            }, true);
            // 根据类型由不同的执行单元处理，
            ushort value = ushort.Parse(Encoding.Default.GetString(e.ApplicationMessage.Payload));
            result_addr.Data.ValueBytes = BitConverter.GetBytes(value).Reverse().ToArray();
            var result = result_eo.Data.Write(new List<CommAddress> { result_addr.Data });
        }
        #endregion

        private ConditionModel CompareValues(List<ConditionModel> conditionList, string operatorStr, object currentValue)
        {
            var query = (from q in conditionList
                         where q.Operator == operatorStr &&
                         Boolean.TryParse(new DataTable().Compute(currentValue + q.Operator + q.CompareValue.ToString(), "").ToString(),
                         out bool state) && state
                         select q).ToList();
            if (query.Count > 1)
            {
                if (operatorStr == "<" || operatorStr == "<=")
                    currentValue = conditionList.Min(v => double.Parse(v.CompareValue));
                else if (operatorStr == ">" || operatorStr == ">=")
                    currentValue = conditionList.Max(v => double.Parse(v.CompareValue));

                return query.FirstOrDefault(v => v.CompareValue == currentValue);
            }
            return query[0];
        }

        private void SetWarning(string key, string message, DeviceItemModel device, string value = "", string vnum = "", int level = 1)
        {
            if (device.IsWarning) return;

            device.IsWarning = true;
            device.WarningMessage = new DeviceAlarmModel
            {
                ANum = "A" + DateTime.Now.ToString("yyyyMMddHHmmssFFF"),
                CNum = key,
                AlarmContent = message,
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Level = level,
                State = 1
            };

            // 保存到数据库
            _localDataAccess.SaveAlarmMessage(new AlarmEntity
            {
                AlarmNum = device.WarningMessage.ANum,
                CNum = key,
                DeviceNum = device.DeviceNum,
                VariableNum = vnum,
                AlarmContent = message,
                RecordTime = device.WarningMessage.DateTime,
                RecordValue = value,
                AlarmLevel = level.ToString(),
                State = "0",
                UserId = GlobalUserInfo.UserName
            });
        }

        private void ComponentsInit()
        {
            var ds = _localDataAccess.GetDevices().Select(d =>
            {
                return new DeviceItemModel(_localDataAccess)
                {
                    IsMonitor = true,
                    X = double.Parse(d.X),
                    Y = double.Parse(d.Y),
                    Z = int.Parse(d.Z),
                    Width = double.Parse(d.W),
                    Height = double.Parse(d.H),
                    DeviceType = d.DeviceTypeName,
                    DeviceNum = d.DeviceNum,
                    FlowDirection = int.Parse(d.FlowDirection),
                    Rotate = int.Parse(d.Rotate),
                    Header = d.Header,

                    PropList = new ObservableCollection<DevicePropModel>(
                        d.Props.Select(dp => new DevicePropModel
                        {
                            PropName = dp.PropName,
                            PropValue = dp.PropValue,
                        })),

                    VariableList = new ObservableCollection<VariableModel>(
                        d.Vars.Select(dv => new VariableModel
                        {
                            DeviceNum = d.DeviceNum,
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
                };
            });
            DeviceList = ds.ToList();

            this.RaisePropertyChanged(nameof(DeviceList));
        }

        private void ShowPage(object obj)
        {
            // Bug：对象会重复创建，需要处理
            // 第80行解决

            var model = obj as MenuModel;
            if (model != null)
            {
                if (GlobalUserInfo.UserType == 0 && model.TargetView != "MonitorPage")
                {
                    // 提示权限
                    this.Menus[0].IsSelected = true;
                    // 提示没有权限操作
                    if (ActionManager.ExecuteAndResult<object>("ShowRight", null))
                    {
                        // 执行重新登录
                        DoLogout();
                    }
                }
                else
                {
                    if (ViewContent != null && ViewContent.GetType().Name == model.TargetView) return;

                    Type type = Assembly.Load("Zhaoxi.DigitaPlatform.Views")
                        .GetType("Zhaoxi.DigitaPlatform.Views.Pages." + model.TargetView)!;
                    ViewContent = Activator.CreateInstance(type)!;
                }
            }
        }

        // 打开配置窗口
        private void ShowConfig()
        {
            if (GlobalUserInfo.UserType > 0)
            {
                this.ViewBlur = 5;
                // 可以打开编辑   启动窗口   主动
                if (ActionManager.ExecuteAndResult<object>("AAA", null))
                {
                    // 添加一个等待页面（预留）

                    // 可能会有耗时控件
                    cts.Cancel();
                    Task.WaitAll(tasks.ToArray());

                    cts = new CancellationTokenSource();
                    tasks.Clear();

                    // 刷新   配置文件/数据库
                    ComponentsInit();
                    // 启动监听
                    this.Monitor();
                }
                this.ViewBlur = 0;
            }
            else
            {
                // 提示没有权限操作
                if (ActionManager.ExecuteAndResult<object>("ShowRight", null))
                {
                    // 执行重新登录
                    DoLogout();
                }
            }
        }

        private void DoLogout()
        {
            Process.Start("Zhaoxi.DigitaPlatform.exe");

            //(obj as Window).Close();
            this.IsWindowClose = true;// 设计上来看  等同于关闭窗口
            // 关闭当前
        }

        public override void Cleanup()
        {
            base.Cleanup();

            cts.Cancel();
            Task.WaitAll(tasks.ToArray());


            //----------断开服务连接----------
            tid++;
            tid %= 0xFFFF;
            List<byte> stopBytes = new List<byte>
            {
                (byte)(tid / 256),
                (byte)(tid % 256),
                (byte)(clientId / 256),
                (byte)(clientId % 256),
                0x07,
                0x00,0x00
            };
            socket.Send(stopBytes.ToArray());
        }
    }
}
