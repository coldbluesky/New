using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class AddAxisYViewModel
    {
        public AxisYModel AxisYModel { get; set; } = new AxisYModel();

        public ICommand ConfirmCommand
        {
            get => new RelayCommand<object>(obj =>
            {
                SimpleIoc.Default.GetInstance<TrendViewModel>().AddAxisY(AxisYModel);
                (obj as Window).DialogResult = true;
            });
        }
    }
}
