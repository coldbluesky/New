using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.DAL
{
    public interface ILoginDataAccess
    {
        DataTable Login(string username, string password);
    }
}
