using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class RecordEntity
    {
        public string DeviceNum { get; set; }
        public string DeviceName { get; set; }
        public string VariableNum { get; set; }
        public string VariableName { get; set; }
        public string RecordValue { get; set; }
        /// <summary>
        /// 记录类型：正常0、报警10、报警已处理1、联控2
        /// </summary>
        public int State { get; set; }
        public string AlarmNum { get; set; }
        public string UnionNum { get; set; }
        public long RecordTime { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string UserId { get; set; }
    }
}
