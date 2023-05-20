using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.DAL;
using Zhaoxi.AirCompression.Model;
using Zhaoxi.AirCompression.View.Pages;

namespace Zhaoxi.AirCompression.BLL
{
    public class AlarmBLL : IAlarmBLL
    {
        IRecordDataAccess _alarmDataAccess;
        public AlarmBLL(IRecordDataAccess alarmDataAccess)
        {
            _alarmDataAccess = alarmDataAccess;
        }
        public List<RecordModel> GetAlarm()
        {
            DataTable datas = _alarmDataAccess.GetAlarmDatas();

            return (from item in datas.AsEnumerable()
                    select new RecordModel
                    {
                        id = (int)item.Field<Int64>("id"),
                        DeviceNum = item.Field<string>("device_num"),
                        DeviceName = item.Field<string>("device_name"),
                        VariableNum = item.Field<string>("var_num"),
                        VariableDesc = item.Field<string>("var_name"),
                        RecordValue = item.Field<string>("record_value"),
                        State = item.Field<Int64>("state").ToString(),
                        DataTime = new DateTime((long)(item.Field<double>("record_time")) * 10000000 + 621355968000000000).ToString("yyyy-MM-dd HH:mm:ss"),
                        UserName = item.Field<string>("user_id"),
                        AlarmNum = item.Field<string>("alarm_num"),
                        AlarmDesc = item.Field<string>("alarm_desc"),
                        AlarmDetail = item.Field<string>("alarm_detail"),
                        AlarmLevel = item.Field<Int64>("alarm_level").ToString()
                    }).ToList();
        }

        public int SetAlarmState(string id, string state)
        {
            return _alarmDataAccess.SetRecordState(id, state);
        }

        public int SetAlarmStateByDevice(string dNum, string state)
        {
            return _alarmDataAccess.SetRecordStateByDevice(dNum, state);
        }
    }
}
