using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.DAL
{
    public class DeviceDataAccess : DataAccessBase, IDeviceDataAccess
    {
        public DataTable GetDevice()
        {
            string sql = "select * from device";
            return this.GetDatas(sql);
        }
        public DataTable GetDeviceById(string deviceNum)
        {
            string sql = $"select * from device where device_num = '{deviceNum}'";
            return this.GetDatas(sql);
        }
        public DataTable GetCommProperties(string deviceNum)
        {
            string sql = $"select * from comm_prop where device_num = '{deviceNum}'";
            return this.GetDatas(sql);
        }

        public DataTable GetVariables(string deviceNum)
        {
            string sql = $"select * from variable where device_num = '{deviceNum}'";
            return this.GetDatas(sql);
        }
        public DataTable GetVariableById(string variableNum)
        {
            string sql = $"select * from variable where var_num = '{variableNum}'";
            return this.GetDatas(sql);
        }
        public DataTable GetAlarms(string varNum)
        {
            string sql = $"select * from alarm where var_num = '{varNum}'";
            return this.GetDatas(sql);
        }

        public DataTable GetUnions(string varNum)
        {
            string sql = $"select * from unions where var_num = '{varNum}'";
            return this.GetDatas(sql);
        }

        List<string> _insertSqls = new List<string>();
        public int Record(List<RecordEntity> records)
        {
            List<string> sqls = new List<string>();
            foreach (var item in records)
            {
                string sql = "insert into record(device_num,device_name,var_num,var_name,record_value,state,alarm_num,union_num,record_time,user_id)" +
                    $" values('{item.DeviceNum}','{item.DeviceName}','{item.VariableNum}','{item.VariableName}','{item.RecordValue}',{item.State},'{item.AlarmNum}','{item.UnionNum}',{item.RecordTime},'{item.UserId}')";
                sqls.Add(sql);
            }

            return this.SqlExecute(sqls);
        }

        public DataTable GetConfig(string id)
        {
            string sql = $"select * from config where config_id = '{id}'";
            return this.GetDatas(sql);
        }
        public int SaveConfig(string id, string device, string variable)
        {
            string sql = $"update config set config_device = '{device}' ";
            if (!string.IsNullOrEmpty(variable))
            {
                sql += $",config_value='{variable}' ";
            }
            sql += $" where config_id = '{id}'";

            int count = this.SqlExecute(new List<string> { sql });
            if (count == 0)
            {
                // 执行插入
                sql = $"insert into config (config_id,config_device,config_value) values('{id}','{device}','{variable}')";
                count = this.SqlExecute(new List<string> { sql });
            }
            return count;
        }

        public int SaveDevice(DeviceModel deviceModel)
        {
            // 保存设备信息
            string sql = $"update device set device_name='{deviceModel.DeviceName}' where device_num='{deviceModel.DeviceNum}'";

            if (this.SqlExecute(new List<string> { sql }) == 0)
            {
                sql = $"insert into device(device_num,device_name,state) values( '{deviceModel.DeviceNum}','{deviceModel.DeviceName}',1)";

                if (this.SqlExecute(new List<string> { sql }) == 0) return 0;
            }

            // 保存通信配置信息
            foreach (var item in deviceModel.CommProperties)
            {
                sql = $"update comm_prop set prop_value='{item.PropValue}' where device_num='{deviceModel.DeviceNum}' and prop_name='{item.PropName}'";

                if (this.SqlExecute(new List<string> { sql }) == 0)
                {
                    sql = $"insert into comm_prop(device_num,prop_name,prop_value) values( '{deviceModel.DeviceNum}','{item.PropName}','{item.PropValue}')";

                    if (this.SqlExecute(new List<string> { sql }) == 0) return 0;
                }
            }

            return 1;
        }

        public int SaveVariable(string deviceNum, VariableModel variableModel)
        {
            // 保存设备信息
            string sql = $"update variable set var_desc='{variableModel.VariableDescription}',var_addr='{variableModel.VarAddress}',var_type='{variableModel.VarType}',operation_type='R',operator='{variableModel.Operator}',offset_value='{variableModel.OffsetValue}' where device_num='{deviceNum}' and var_num='{variableModel.VariableNum}'";

            if (this.SqlExecute(new List<string> { sql }) == 0)
            {
                sql = $"insert into variable(device_num,var_num,var_desc,var_addr,var_type,operation_type,operator,offset_value) values( '{deviceNum}','{variableModel.VariableNum}','{variableModel.VariableDescription}','{variableModel.VarAddress}','{variableModel.VarType}','R','{variableModel.Operator}','{variableModel.OffsetValue}')";

                if (this.SqlExecute(new List<string> { sql }) == 0) return 0;
            }
            return 1;
        }
    }
}
