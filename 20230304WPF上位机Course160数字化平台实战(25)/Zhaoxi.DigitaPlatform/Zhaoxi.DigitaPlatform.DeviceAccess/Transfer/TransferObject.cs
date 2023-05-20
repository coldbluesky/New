using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.DeviceAccess.Transfer
{
    internal abstract class TransferObject
    {
        public object TUnit { get; set; }

        internal List<string> Conditions = new List<string>();

        internal virtual Result Config(List<DevicePropItemEntity> props) => new Result();

        internal virtual Result Connect(int trycount = 30) => new Result();

        internal virtual Result<List<byte>> SendAndReceived(List<byte> req, int len1, int len2, int timeout) => null;

        // 参数：calcLen
        // 委托，作用是
        internal virtual Result<List<byte>> SendAndReceived(List<byte> req, int len1, int timeout, Func<byte[], int> calcLen = null)
            => new Result<List<byte>>(false, "NULL");


        internal bool ConnectState { get; set; } = false;
        internal virtual Result Close()
        {
            this.ConnectState = false;
            return new Result();
        }
    }
}
