using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zhaoxi.AirCompression.Base.Languages;
using Zhaoxi.AirCompression.BLL;
using Zhaoxi.AirCompression.Model;

namespace Zhaoxi.AirCompression.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _currentLangName;

        public string CurrentLangName
        {
            get { return _currentLangName; }
            set
            {
                Set<string>(ref _currentLangName, value);
                LangProvider.CurrentLang = value;
            }
        }

        public List<LangListItem> Langs { get; set; } = new List<LangListItem>();



        ILoginBLL _loginBLL;
        public LoginViewModel(ILoginBLL loginBLL)
        {
            _loginBLL = loginBLL;

            Langs.Add(new LangListItem { Key = "zh-cn", DisplayName = "中文" });
            Langs.Add(new LangListItem { Key = "en-us", DisplayName = "English" });
        }

        public UserModel UserModel { get; set; } = new UserModel();
        private string _errorMsg;

        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { Set<string>(ref _errorMsg, value); }
        }

        public ICommand LoginCommand
        {
            get => new RelayCommand<object>(win =>
            {
                try
                {
                    this.ErrorMsg = "";
                    if (string.IsNullOrEmpty(this.UserModel.UserName))
                    {
                        //throw new Exception("用户名不能为空！请输入用户名");
                        throw new Exception(LangProvider.LoginUserError);
                    }
                    if (string.IsNullOrEmpty(this.UserModel.Password))
                    {
                        throw new Exception("密码不能为空！请输入密码");
                    }
                    if (_loginBLL.Login(UserModel.UserName, UserModel.Password))
                        (win as System.Windows.Window).DialogResult = true;
                    else
                    {
                        this.UserModel.Password = "";
                        throw new Exception("用户名或密码错误");
                    }
                }
                catch (Exception ex)
                {
                    this.ErrorMsg = LangProvider.LoginExceptionHeader + ex.Message;
                }
            });
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
