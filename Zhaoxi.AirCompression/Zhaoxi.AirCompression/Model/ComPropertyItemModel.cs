using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class ComPropertyItemModel : ObservableObject
    {
        public string PropName { get; set; }
        public string PropKey { get; set; }

        private List<string> _selectValues;

        public List<string> SelectValues
        {
            get { return _selectValues; }
            set { Set(ref _selectValues, value); }
        }
    }
}
