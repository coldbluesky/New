using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class DataGridColumnModel : ObservableObject
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }
        /// <summary>
        /// 添加的顺序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 列头标签文字
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// 绑定的属性
        /// </summary>
        public string BindingPath { get; set; }
        /// <summary>
        /// 列宽
        /// </summary>
        public int ColumnWidth { get; set; }
    }
}
