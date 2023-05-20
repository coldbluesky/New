using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Zhaoxi.AirCompression.Base.Languages
{
    public class LangProvider
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        internal static ResourceDictionary langRes = new ResourceDictionary()
        {
            Source = new Uri($"pack://application:,,,/Zhaoxi.AirCompression;component/Base/Languages/Lang.en-us.xaml", UriKind.RelativeOrAbsolute)
        };

        public static string _currentLang = "zh-cn";
        public static string CurrentLang
        {
            get => _currentLang;
            set
            {
                _currentLang = value;
                string uriStr = $"pack://application:,,,/Zhaoxi.AirCompression;component/Base/Languages/Lang.{value}.xaml";
                langRes.Source = new Uri(uriStr, UriKind.RelativeOrAbsolute);

                // 属性变化通知
                Assembly assembly = Assembly.Load("Zhaoxi.AirCompression");
                Type type = assembly.GetType("Zhaoxi.AirCompression.Base.Languages.LangProvider");
                PropertyInfo[] pis = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
                foreach (var pi in pis)
                {
                    if (pi.IsDefined(typeof(NotifyAttribute)))
                        StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(pi.Name));
                }
            }
        }


        // 变化的时候需要一个通知
        [Notify]
        public static string SystemName { get => langRes["SystemName"].ToString(); }
        [Notify]
        public static string Login { get => langRes["Login"].ToString(); }
        [Notify]
        public static string EnterMask_UN { get => langRes["EnterMask_UN"].ToString(); }
        [Notify]
        public static string EnterMask_PWD { get => langRes["EnterMask_PWD"].ToString(); }


        /// <summary>
        /// 登录逻辑里调用的字符串
        /// </summary>
        public static string LoginExceptionHeader { get => langRes["LoginExceptionHeader"].ToString(); }
        public static string LoginUserError { get => langRes["LoginUserError"].ToString(); }

    }

    public class LangListItem
    {
        public LangListItem() { }
        // 显示中文、English
        // 获取zh-cn   en-us
        public LangListItem(string key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }
        public string Key { get; set; }
        public string DisplayName { get; set; }
    }

    class NotifyAttribute : Attribute { }
}
