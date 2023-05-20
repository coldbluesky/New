using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.DigitaPlatform.Common;
using Zhaoxi.DigitaPlatform.Models;

namespace Zhaoxi.DigitaPlatform.ViewModels
{
    public class ConditionDialogViewModel : ViewModelBase
    {
        public List<ConditionOperatorModel> Operators { get; set; } = new List<ConditionOperatorModel>();

        public List<DeviceDropModel> DeviceDropList { get; set; }

        public ConditionDialogViewModel()
        {
            if (!IsInDesignMode)
            {
                // 只处理基本的逻辑运算     扩展：组件逻辑处理   &&   ||    ()
                Operators.Add(new ConditionOperatorModel() { Header = "大于", Value = ">" });
                Operators.Add(new ConditionOperatorModel() { Header = "小于", Value = "<" });
                Operators.Add(new ConditionOperatorModel() { Header = "等于", Value = "==" });
                Operators.Add(new ConditionOperatorModel() { Header = "大于等于", Value = ">=" });
                Operators.Add(new ConditionOperatorModel() { Header = "小于等于", Value = "<=" });
                Operators.Add(new ConditionOperatorModel() { Header = "不等于", Value = "!=" });

                // 关于获取设备列表，有几种方式：
                /// 1、类似于数据聚合器，
                ///    - 主动发送一个请求，对方 接收到后，再响应一个请求（）
                ///    - 只需要一个请求，不需要回调；Action  Func
                /// 2、需要设备列表，MainViewModel/ConfigViewModel，把这其中一个注入到这个VM里呢？

                // 事件聚合器
                // ActionManager.Execute("GetDevice", "Data");
                ActionManager.Execute(
                    "GetDevice",
                    DoGet
                    );
            }
        }
        private void DoGet(Func<List<DeviceDropModel>> func)
        {
            DeviceDropList = func.Invoke();
        }
    }
}
