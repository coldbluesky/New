using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class ModifyVariableViewModel
    {
        public string DeviceNum { get; set; }
        public VariableModel Variable { get; set; }

        public List<OperatorItemModel> Operators { get; set; } = new List<OperatorItemModel>();
        public List<VariableTypeModel> VariableTypes { get; set; } = new List<VariableTypeModel>();

        public ICommand ConfirmCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                // 保存数据
                if (_deviceBLL.SaveVariable(DeviceNum, Variable) > 0)
                {
                    SimpleIoc.Default.GetInstance<MainViewModel>().Devices.Get(DeviceNum)?.RefreshVariable(Variable);
                    (obj as Window).DialogResult = true;
                }
            });
        }

        IDeviceBLL _deviceBLL;
        public ModifyVariableViewModel(IDeviceBLL deviceBLL)
        {
            _deviceBLL = deviceBLL;


            #region 初始化偏移运算符
            Operators.Add(new OperatorItemModel { Operator = "", OperatorText = "未选择" });
            Operators.Add(new OperatorItemModel { Operator = "+", OperatorText = "加" });
            Operators.Add(new OperatorItemModel { Operator = "-", OperatorText = "减" });
            Operators.Add(new OperatorItemModel { Operator = "*", OperatorText = "乘" });
            Operators.Add(new OperatorItemModel { Operator = "/", OperatorText = "除" });
            #endregion

            #region 初始化数据类型
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.UInt16" });
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.Int16" });
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.UInt32" });
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.Int32" });
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.Single" });
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.UInt64" });
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.Int64" });
            VariableTypes.Add(new VariableTypeModel { TypeName = "System.Boolean" });
            #endregion
        }
    }
}