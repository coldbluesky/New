using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Zhaoxi.DigitaPlatform.DataAccess;
using Zhaoxi.DigitaPlatform.Entities;
using Zhaoxi.DigitaPlatform.IDataAccess;

namespace Zhaoxi.DigitaPlatform.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ConfigViewModel>();
            SimpleIoc.Default.Register<ConditionDialogViewModel>();
            SimpleIoc.Default.Register<TrendViewModel>();
            SimpleIoc.Default.Register<AlarmViewModel>();
            SimpleIoc.Default.Register<ReportViewModel>();
            SimpleIoc.Default.Register<SettingsViewMdole>();

            SimpleIoc.Default.Register<ILocalDataAccess, LocalDataAccess>();

        }

        // 这种属性定义方式会有歧义，感觉好像定义的字段
        public LoginViewModel LoginViewModel => ServiceLocator.Current.GetInstance<LoginViewModel>();
        // 上面写法等同于下面完整处理
        //public LoginViewModel value
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<LoginViewModel>();
        //    }
        //}
        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();
        public ConfigViewModel ConfigViewModel => ServiceLocator.Current.GetInstance<ConfigViewModel>();
        public ConditionDialogViewModel ConditionDialogViewModel => ServiceLocator.Current.GetInstance<ConditionDialogViewModel>();
        public TrendViewModel TrendViewModel => ServiceLocator.Current.GetInstance<TrendViewModel>();
        public AlarmViewModel AlarmViewModel => ServiceLocator.Current.GetInstance<AlarmViewModel>();
        public ReportViewModel ReportViewModel => ServiceLocator.Current.GetInstance<ReportViewModel>();
        public SettingsViewMdole SettingsViewMdole => ServiceLocator.Current.GetInstance<SettingsViewMdole>();


        // 通过ViewModelLocator对象实例 进行对应的VM对象的销毁
        public static void Cleanup<T>() where T : ViewModelBase
        {
            if (SimpleIoc.Default.IsRegistered<T>() && SimpleIoc.Default.ContainsCreated<T>())
            {
                var instances = SimpleIoc.Default.GetAllCreatedInstances<T>();
                foreach (var instance in instances)
                {
                    instance.Cleanup();
                }
                // 
                SimpleIoc.Default.Unregister<T>();
                SimpleIoc.Default.Register<T>();
            }
        }
        //public static void CleanupAll()
        //{
        //    foreach (var vmt in vmTypes)
        //    {
        //        var vms = SimpleIoc.Default.GetAllCreatedInstances(vmt);
        //        foreach (var item in vms)
        //        {
        //            (item as ViewModelBase).Cleanup();
        //        }
        //    }
        //    SimpleIoc.Default.Reset();
        //}


        private static List<RecordWriteEntity> records = new List<RecordWriteEntity>();

        public static void AddRecord(RecordWriteEntity record)
        {
            if (record != null && record != null)
                records.Add(record);
            if (record == null || records.Count >= 200)
            {
                ServiceLocator.Current.GetInstance<ILocalDataAccess>().SaveRecord(records);
                records.Clear();
            }
        }

        // 分库分表
    }
}
