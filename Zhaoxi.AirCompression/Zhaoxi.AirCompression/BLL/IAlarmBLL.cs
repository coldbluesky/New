using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.BLL
{
    public interface IAlarmBLL
    {
        List<RecordModel> GetAlarm();
        int SetAlarmState(string id, string state);
        int SetAlarmStateByDevice(string dNum, string state);
    }
}
