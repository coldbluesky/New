using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.Components
{
    public class DeviceCollection : IEnumerable, INotifyCollectionChanged
    {
        int index = 0;
        //保存Object 类型的 数组。
        List<DeviceModel> dms = new List<DeviceModel>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DeviceCollection() { }
        public DeviceCollection(List<DeviceModel> deviceModels)
        {
            dms.AddRange(deviceModels);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in dms)
            {
                yield return item;
            }
        }

        public DeviceModel Next()
        {
            if (index >= 0 && dms.Count > 0 && index < dms.Count)
            {
                var result = dms[index++];
                if (index >= dms.Count)
                    index = 0;
                return result;
            }
            return null;
        }
        public DeviceModel Get(string deviceNum)
        {
            return dms.FirstOrDefault(d => d.DeviceNum == deviceNum);
        }

        // 传设备的ID
        public DeviceModel this[string key]
        {
            // 未考虑空对象的情况处理
            get => this.dms.FirstOrDefault(d => d.DeviceNum == key);
        }

        public void Add(DeviceModel deviceModel)
        {
            this.dms.Add(deviceModel);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Remove(string deviceNum)
        {
            this.dms.RemoveAll(d => d.DeviceNum == deviceNum);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
