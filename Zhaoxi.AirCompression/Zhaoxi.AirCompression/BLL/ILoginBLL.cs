using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.BLL
{
    public interface ILoginBLL
    {
        bool Login(string username, string password);
    }
}
