using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.DigitaPlatform.Models
{
    // SelectedItem
    public class PropOptionModel
    {
        // 用来显示  "串口名称" 
        public string Header { get; set; }
        // 用来保存  "PortName"
        public string PropName { get; set; }
        // 属性的操作类型   0表示键盘输入   1表示下拉选择
        public int PropType { get; set; }

        public List<string> ValueOptions { get; set; }
        public int DefaultIndex { get; set; }
    }
}
