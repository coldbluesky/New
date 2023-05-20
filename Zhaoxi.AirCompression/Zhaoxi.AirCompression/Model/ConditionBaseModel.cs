using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class ConditionBaseModel
    {
        public string VariableNum { get; set; }
        public string ConditionNum { get; set; }
        public string Operator { get; set; }
        public double CompareValue { get; set; }
    }
}
