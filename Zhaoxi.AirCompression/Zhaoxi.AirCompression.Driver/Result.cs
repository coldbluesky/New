using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Driver
{
    public class Result<T>
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsSuccessed { get; set; }
        /// <summary>
        /// 对应的消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Datas { get; set; }


        public Result() : this(true, "OK") { }
        public Result(bool state, string msg) : this(state, msg, new List<T>()) { }
        public Result(bool state, string msg, List<T> datas)
        {
            this.IsSuccessed = state; Message = msg; Datas = datas;
        }
    }

    public class Result : Result<bool>
    {
        public Result() : base(true, "OK") { }
        public Result(bool state, string msg) : base(state, msg) { }
    }
}
