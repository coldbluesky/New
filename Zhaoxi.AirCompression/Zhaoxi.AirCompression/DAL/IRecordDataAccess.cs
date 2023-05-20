using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.DAL
{
    public interface IRecordDataAccess
    {
        DataTable GetAlarmDatas();
        DataTable GetRecords(string condition = "");

        int SetRecordState(string id, string vNum);
        int SetRecordStateByDevice(string dNum, string vNum);
    }
}
