using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.DAL
{
    public class LoginDataAccess : DataAccessBase, ILoginDataAccess
    {
        public DataTable Login(string username, string password)
        {
            string userSql = "select * from users where user_name=@user_name and password=@password";

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@user_name", username);
            param.Add("@password", password);

            return this.GetDatas(userSql, param);
        }
    }
}
