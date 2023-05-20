﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.DigitaPlatform.Models
{
    public class ConditionModel
    {
        // 作用后续报警重复提示的区分
        public string CNum { get; set; }
        public string Operator { get; set; }
        public string CompareValue { get; set; }
        public string AlarmContent { get; set; }

        public ObservableCollection<UnionDeviceModel> UnionDeviceList { get; set; } =
            new ObservableCollection<UnionDeviceModel>();

        public RelayCommand AddDeviceCommand
        {
            get => new RelayCommand(() =>
            {
                UnionDeviceList.Add(new UnionDeviceModel() { ValueType = "UInt16", UNum = "U" + DateTime.Now.ToString("yyyyMMddHHmmssFFFF") });
            });
        }
        public RelayCommand<UnionDeviceModel> DelDeviceCommand
        {
            get => new RelayCommand<UnionDeviceModel>(model =>
            {
                UnionDeviceList.Remove(model);
            });
        }
    }
}
