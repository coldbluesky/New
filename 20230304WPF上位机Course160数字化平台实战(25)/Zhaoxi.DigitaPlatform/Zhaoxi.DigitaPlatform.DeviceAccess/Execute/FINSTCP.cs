using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Transfer;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Execute
{
    internal class FINSTCP : ExecuteObject
    {
        internal override Result Match(List<DevicePropItemEntity> props, List<TransferObject> tos)
        {
            var ps = props.Where(p => p.PropName == "IP" || p.PropName == "Port").Select(p => p.PropValue).ToList();
            return this.Match(props, tos, ps, "SocketTcpUnit");
        }

        public override Result Read(List<CommAddress> variables)
        {
            return base.Read(variables);
        }

        public override Result<List<CommAddress>> GroupAddress(List<VariableProperty> variables)
        {
            return base.GroupAddress(variables);
        }
    }
}
