using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class UnionModel : ConditionBaseModel
    {
        public string ExeDeviceNum { get; set; }
        public string ExeVarNum { get; set; }
        public string InputVar { get; set; }
        public string InputType { get; set; }
    }
}
