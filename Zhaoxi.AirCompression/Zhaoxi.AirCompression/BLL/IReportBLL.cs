using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.BLL
{
    public interface IReportBLL
    {
        List<RecordModel> GetRecords();
    }
}
