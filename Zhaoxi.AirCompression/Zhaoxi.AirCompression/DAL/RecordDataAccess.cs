using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.DAL
{
    public class RecordDataAccess : DataAccessBase, IRecordDataAccess
    {
        public DataTable GetAlarmDatas()
        {
            return this.GetRecords("state = 10 or state = 1");
        }
        public DataTable GetRecords(string condition = "")
        {
            string sql = "select a.id,a.device_num,a.device_name,a.var_num,a.var_name,a.record_value,a.state,a.record_time,a.user_id,b.alarm_num,b.alarm_desc,b.alarm_detail,IfNULL(b.alarm_level,0) alarm_level from record a LEFT JOIN alarm b on a.alarm_num = b.alarm_num where 1=1";
            if (!string.IsNullOrEmpty(condition)) sql += " and " + condition;
            //"a.state = 10 or a.state = 1";
            return this.GetDatas(sql);
        }

        public int SetRecordState(string id, string state)
        {
            string sql = $"update record set state = {state} where id={id}";
            return this.SqlExecute(new List<string> { sql });
        }

        public int SetRecordStateByDevice(string dNum, string state)
        {
            string sql = $"update record set state = {state} where device_num={dNum}";
            return this.SqlExecute(new List<string> { sql });
        }
    }
}
