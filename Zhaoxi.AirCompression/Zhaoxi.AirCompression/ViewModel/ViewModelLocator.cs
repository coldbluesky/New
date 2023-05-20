using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.DAL;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            if (ViewModelBase.IsInDesignModeStatic)
            {

            }
            else
            {

            }
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SimulatorViewModel>();
            SimpleIoc.Default.Register<ConfigViewModel>();
            SimpleIoc.Default.Register<TrendViewModel>();
            SimpleIoc.Default.Register<ReportViewModel>();
            SimpleIoc.Default.Register<AlarmViewModel>();


            SimpleIoc.Default.Register<ChooseDeviceViewModel>();
            SimpleIoc.Default.Register<ModifyDeviceViewModel>();
            SimpleIoc.Default.Register<ModifyVariableViewModel>();
            SimpleIoc.Default.Register<AddAxisYViewModel>();
        }

        public MainViewModel Main { get => ServiceLocator.Current.GetInstance<MainViewModel>(); }
        public LoginViewModel Login { get => ServiceLocator.Current.GetInstance<LoginViewModel>(); }
        public SimulatorViewModel Simulator { get => ServiceLocator.Current.GetInstance<SimulatorViewModel>(); }
        public ConfigViewModel Config { get => ServiceLocator.Current.GetInstance<ConfigViewModel>(); }
        public TrendViewModel Trend { get => ServiceLocator.Current.GetInstance<TrendViewModel>(); }
        public ReportViewModel Report { get => ServiceLocator.Current.GetInstance<ReportViewModel>(); }
        public AlarmViewModel Alarm { get => ServiceLocator.Current.GetInstance<AlarmViewModel>(); }


        public ModifyDeviceViewModel ModifyDevice { get => ServiceLocator.Current.GetInstance<ModifyDeviceViewModel>(); }
        public ModifyVariableViewModel ModifyVariable { get => ServiceLocator.Current.GetInstance<ModifyVariableViewModel>(); }
        public AddAxisYViewModel AddAxisY { get => ServiceLocator.Current.GetInstance<AddAxisYViewModel>(); }


        public ChooseDeviceViewModel ChooseDevice { get => ServiceLocator.Current.GetInstance<ChooseDeviceViewModel>(); }


        public static void Cleanup<T>() where T : ViewModelBase
        {
            if (SimpleIoc.Default.IsRegistered<T>())
            {
                ServiceLocator.Current.GetInstance<T>().Cleanup();

                SimpleIoc.Default.Unregister<T>();
                SimpleIoc.Default.Register<T>();
            }
        }
    }
}
