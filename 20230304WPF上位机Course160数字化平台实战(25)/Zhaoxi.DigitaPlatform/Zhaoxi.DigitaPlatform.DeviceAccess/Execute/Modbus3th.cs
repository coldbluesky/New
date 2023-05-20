using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.DeviceAccess.Transfer;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Execute
{
    internal class ModbusRTU3th : ExecuteObject
    {
        Modbus.Device.IModbusSerialMaster master;

        internal override Result Match(List<DevicePropItemEntity> props, List<TransferObject> tos)
        {
            var ps = props.Where(p => p.PropName == "PortName").Select(p => p.PropValue).ToList();
            var result = this.Match(props, tos, ps, "SerialUnit");
            master = Modbus.Device.ModbusSerialMaster.CreateRtu(this.TransferObject.TUnit as SerialPort);
            return result;
        }

        public override Result Read(List<CommAddress> variables)
        {
            var prop = this.Props.FirstOrDefault(p => p.PropName == "TryCount");
            int tryCount = 30;
            if (prop != null)
                int.TryParse(prop.PropValue, out tryCount);
            Result connectState = this.TransferObject.Connect(tryCount);
            if (!connectState.Status)
                throw new Exception(connectState.Message);


            //master.ReadHoldingRegisters()
            return base.Read(variables);
        }
    }
}
