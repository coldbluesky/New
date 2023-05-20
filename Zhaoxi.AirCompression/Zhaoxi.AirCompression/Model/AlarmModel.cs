using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class AlarmModel: ConditionBaseModel
    {
        //public string AlarmNum { get; set; }
        //public string VariableNum { get; set; }
        //public string Operator { get; set; }
        //public double CompareValue { get; set; }

        public string AlarmDesc { get; set; }
        public string AlarmNote { get; set; }
        public int AlarmLevel { get; set; }
    }
}
