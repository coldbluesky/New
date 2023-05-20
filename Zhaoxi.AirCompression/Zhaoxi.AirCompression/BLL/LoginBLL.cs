using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AirCompression.Base;
using Zhaoxi.AirCompression.DAL;

namespace Zhaoxi.AirCompression.BLL
{
    public class LoginBLL : ILoginBLL
    {
        ILoginDataAccess _loginDataAccess;
        public LoginBLL(ILoginDataAccess loginDataAccess)
        {
            _loginDataAccess = loginDataAccess;
        }

        public bool Login(string username, string password)
        {
            // 1、获取对应的用户详细信息
            // 2、返回是否通过

            string pwd = Md5Provider.GetMD5String(Md5Provider.GetMD5String(password) + "|" + username);
            DataTable dataTable = _loginDataAccess?.Login(username, pwd);

            if (dataTable == null || dataTable.AsEnumerable().Count() == 0) return false;

            // 记录全局用户信息
            GlobalValues.UserInfo = new Model.UserModel
            {
                UserName = username,
                Password = pwd
            };

            return true;
        }
    }
}
