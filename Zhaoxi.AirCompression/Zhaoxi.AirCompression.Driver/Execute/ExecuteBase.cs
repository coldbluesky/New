using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Driver.Base;
using Zhaoxi.AirCompression.Driver.Component;

namespace Zhaoxi.AirCompression.Driver.Execute
{
    public abstract class ExecuteBase
    {
        // 这里的方法一般都提供给外部使用

        // 可能会有公共的执行过程

        public int Timeout = 2000;


        // 虚方法
        public virtual Result BuildCommunication(IComponent component, CommProperty commProperty) { return new Result(); }
        public virtual Result Read(List<PointProperty> address, IComponent component, CommProperty commProperty) { return null; }
        public virtual void ReadAsync(List<PointProperty> points, IComponent component, CommProperty commProperty, Action<Result> callback) { }
        public virtual Result Write(List<PointProperty> points, IComponent component, CommProperty commProperty) { return null; }
        public virtual void WriteAsync(List<PointProperty> points, IComponent component, CommProperty commProperty, Action<Result> callback) { }
    }
}
