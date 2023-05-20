using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using Zhaoxi.DigitaPlatform.IDataAccess;
using Zhaoxi.DigitaPlatform.Models;

namespace Zhaoxi.DigitaPlatform.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public UserModel User { get; set; }

        public RelayCommand<object> LoginCommand { get; set; }

        public string _failedMsg;
        public string FailedMsg
        {
            get { return _failedMsg; }
            set { Set(ref _failedMsg, value); }
        }

        ILocalDataAccess _localDataAccess;
        public LoginViewModel(ILocalDataAccess localDataAccess)
        {
            _localDataAccess = localDataAccess;
            if (!IsInDesignMode)
            {
                User = new UserModel();
                LoginCommand = new RelayCommand<object>(DoLogin);
            }
        }
        private void DoLogin(object obj)
        {
            // �Խ����ݿ�
            try
            {
                var data = _localDataAccess.Login(User.UserName, User.Password);
                if (data == null) throw new Exception("��¼ʧ�ܣ�û���û���Ϣ");

                // ��¼һ������������Ҫ���û���Ϣ������SimpleIOC   ͬһ��ʵ����Ĭ���ǵ���
                var main = ServiceLocator.Current.GetInstance<MainViewModel>();
                if (main != null)
                {
                    main.GlobalUserInfo.UserName = User.UserName;
                    main.GlobalUserInfo.Password = User.Password;
                    main.GlobalUserInfo.RealName = data.Rows[0]["real_name"].ToString()!;
                    main.GlobalUserInfo.UserType = int.Parse(data.Rows[0]["user_type"].ToString()!);
                    main.GlobalUserInfo.Gender = int.Parse(data.Rows[0]["gender"].ToString()!);
                    main.GlobalUserInfo.Department = data.Rows[0]["department"].ToString()!;
                    main.GlobalUserInfo.PhoneNumber = data.Rows[0]["phone_num"].ToString()!;
                }
                (obj as Window).DialogResult = true;
            }
            catch (Exception ex)
            {
                FailedMsg = ex.Message;
            }
        }
    }
}
