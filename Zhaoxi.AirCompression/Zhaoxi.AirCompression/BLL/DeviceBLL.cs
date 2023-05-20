using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.DAL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.BLL
{
    public class DeviceBLL : IDeviceBLL
    {
        IDeviceDataAccess _dataAccess;
        public DeviceBLL(IDeviceDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<DeviceModel> GetDevice()
        {
            List<DeviceModel> devices = new List<DeviceModel>();

            // 获取设备信息
            DataTable d_datas = _dataAccess.GetDevice();
            foreach (var d_item in d_datas.AsEnumerable())
            {
                DeviceModel device = new DeviceModel();
                device.DeviceNum = d_item.Field<string>("device_num");
                device.DeviceName = d_item.Field<string>("device_name");
                device.State = (int)d_item.Field<Int64>("state");
                devices.Add(device);

                // 根据DeviceNum获取通信配置信息
                DataTable comm_datas = _dataAccess.GetCommProperties(device.DeviceNum);
                foreach (var item in comm_datas.AsEnumerable())
                {
                    CommPropertyModel entity = new CommPropertyModel();
                    entity.PropName = item.Field<string>("prop_name");
                    entity.PropValue = item.Field<string>("prop_value");

                    if (device.CommProperties == null)
                        device.CommProperties = new System.Collections.ObjectModel.ObservableCollection<CommPropertyModel>();

                    device.CommProperties.Add(entity);
                }

                // 根据DeviceNum获取点位配置信息
                DataTable var_datas = _dataAccess.GetVariables(device.DeviceNum);
                foreach (var v_item in var_datas.AsEnumerable())
                {
                    VariableModel entity = new VariableModel()
                    {
                        DeivceNum = device.DeviceNum,
                        VariableNum = v_item.Field<string>("var_num"),
                        VariableDescription = v_item.Field<string>("var_desc"),
                        VarAddress = v_item.Field<string>("var_addr"),
                        VarType = v_item.Field<string>("var_type"),
                        OperationType = v_item.Field<string>("operation_type"),
                        Operator = v_item.Field<string>("operator"),
                        OffsetValue = v_item.Field<string>("offset_value")
                    };
                    if (device.Variables == null)
                        device.Variables = new ObservableCollection<VariableModel>();

                    device.Variables.Add(entity);

                    /// 通信点位编号获取对应的报警配置信息
                    DataTable a_datas = _dataAccess.GetAlarms(entity.VariableNum);
                    foreach (var a_item in a_datas.AsEnumerable())
                    {
                        AlarmModel alarmModel = new AlarmModel();
                        alarmModel.ConditionNum = a_item.Field<string>("alarm_num");
                        alarmModel.VariableNum = a_item.Field<string>("var_num");
                        alarmModel.Operator = a_item.Field<string>("operator");
                        alarmModel.CompareValue = double.Parse(a_item.Field<string>("compare_value"));
                        alarmModel.AlarmDesc = a_item.Field<string>("alarm_desc");
                        alarmModel.AlarmNote = a_item.Field<string>("alarm_detail");
                        alarmModel.AlarmLevel = (int)a_item.Field<Int64>("alarm_level");
                        if (entity.AlarmConditions == null)
                            entity.AlarmConditions = new List<AlarmModel>();
                        entity.AlarmConditions.Add(alarmModel);
                    }

                    // 根据DeviceNum获取点位配置信息
                    DataTable u_datas = _dataAccess.GetUnions(entity.VariableNum);
                    foreach (var u_item in u_datas.AsEnumerable())
                    {
                        UnionModel unionModel = new UnionModel();
                        unionModel.ConditionNum = u_item.Field<string>("union_num");
                        unionModel.VariableNum = u_item.Field<string>("var_num");
                        unionModel.Operator = u_item.Field<string>("operator");
                        unionModel.CompareValue = double.Parse(u_item.Field<string>("compare_value"));


                        unionModel.ExeDeviceNum = u_item.Field<string>("exe_device_num");
                        unionModel.ExeVarNum = u_item.Field<string>("exe_var_num");
                        unionModel.InputVar = u_item.Field<string>("input_var");
                        unionModel.InputType = u_item.Field<string>("input_type");

                        if (entity.UnionConditions == null)
                            entity.UnionConditions = new List<UnionModel>();

                        entity.UnionConditions.Add(unionModel);
                    }

                    entity.AlarmNotify = info =>
                    {
                        device.State = 3;
                        device.AlarmInfo = info;
                    };
                    entity.SaveRecord = model =>
                    {
                        model.DeviceNum = device.DeviceNum;
                        model.DeviceName = device.DeviceName;
                        // 等待多条记录时再一起提交 ，可以使用事务
                        // 事务Commit之前   
                        // 100条提交
                        ///如何解决收集中途异常退出的问题？

                        // 数据量大了之后出现效率低下？  Sqlite单文件数据
                        // 1、记录表做分离
                        //    基本配置信息做一个基础库（用户、设备、通信、点位、报警、联控）
                        // 2、建立记录表模板   单独的Sqlite文件，里面放一个表：Record
                        //    判断一下时间，当前加载的Sqlite文件名称跟当前时间是否一致，
                        ///                 如果不一致，就复制一份，文件名称就是时间标记
                        ///3、数据联查
                        ///   Sqlite可以做数据附加加载
                        ///       附加：attach database '202108.db' sa DB202108
                        ///             select * from DB202108.record
                        ///       分离：detach database DB202108
                        ///       
                        recordModels.Add(model);
                        if (recordModels.Count >= 100)
                        {
                            int count = _dataAccess.Record(recordModels);
                            if (count == recordModels.Count)
                            {
                                //提交成功
                                recordModels.Clear();
                            }
                        }
                    };
                }
            }


            return devices;
        }

        public List<DeviceModel> GetDeviceList()
        {
            List<DeviceModel> devices = new List<DeviceModel>();

            // 获取设备信息
            DataTable d_datas = _dataAccess.GetDevice();
            foreach (var d_item in d_datas.AsEnumerable())
            {
                DeviceModel device = new DeviceModel();
                device.DeviceNum = d_item.Field<string>("device_num");
                device.DeviceName = d_item.Field<string>("device_name");
                devices.Add(device);
            }
            return devices;
        }


        List<RecordEntity> recordModels = new List<RecordEntity>();

        public DeviceModel GetDeviceAndComm(string deviceNum)
        {
            DeviceModel device = new DeviceModel();
            DataTable d_datas = _dataAccess.GetCommProperties(deviceNum);
            if (d_datas != null && d_datas.Rows.Count > 0)
            {
                device.DeviceNum = deviceNum;

                foreach (var item in d_datas.AsEnumerable())
                {
                    CommPropertyModel entity = new CommPropertyModel();
                    entity.PropName = item.Field<string>("prop_name");
                    entity.PropValue = item.Field<string>("prop_value");

                    if (device.CommProperties == null)
                        device.CommProperties = new System.Collections.ObjectModel.ObservableCollection<CommPropertyModel>();

                    device.CommProperties.Add(entity);
                }
            }
            return device;
        }

        public VariableModel GetVariableById(string varNum)
        {
            DataTable v_datas = _dataAccess.GetVariableById(varNum);
            if (v_datas != null && v_datas.Rows.Count > 0)
            {
                return new VariableModel()
                {
                    VariableNum = v_datas.Rows[0].Field<string>("var_num"),
                    VariableDescription = v_datas.Rows[0].Field<string>("var_desc"),
                    VarAddress = v_datas.Rows[0].Field<string>("var_addr"),
                    VarType = v_datas.Rows[0].Field<string>("var_type"),
                    OperationType = v_datas.Rows[0].Field<string>("operation_type"),
                    Operator = v_datas.Rows[0].Field<string>("operator"),
                    OffsetValue = v_datas.Rows[0].Field<string>("offset_value")
                };
            }
            return null;
        }

        public ConfigModel GetConfig(string key)
        {
            var data = _dataAccess.GetConfig(key);
            if (data != null && data.AsEnumerable().Count() > 0)
            {
                return new ConfigModel
                {
                    ConfigId = data.Rows[0].Field<string>("config_id"),
                    ConfigDevice = data.Rows[0].Field<string>("config_device"),
                    ConfigValue = data.Rows[0].Field<string>("config_value"),
                };
            }

            return null;
        }

        public int SaveConfig(string key, string device, string variable = "")
        {
            return _dataAccess.SaveConfig(key, device, variable);
        }

        public int SaveDevice(DeviceModel deviceModel)
        {
            return _dataAccess.SaveDevice(deviceModel);
        }

        public int SaveVariable(string deviceNum, VariableModel variableModel)
        {
            return _dataAccess.SaveVariable(deviceNum, variableModel);
        }
    }
}
