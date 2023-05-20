using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Zhaoxi.DigitaPlatform.ViewModels;

namespace Zhaoxi.DigitaPlatform
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnExit(ExitEventArgs e)
        {
            //base.OnExit(e);
            ViewModelLocator.AddRecord(null);
        }
    }






    class A
    {
        private A() { }

        private static A _instance;
        private static readonly object _instanceLock = new object();

        public static A GetInstance()
        {
            if (_instance == null)
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new A();
                }
            }
            return _instance;
        }
    }
}
