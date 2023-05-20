using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;
using HslCommunication;
using HslCommunication.Profinet.Omron;
//using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using Spire.Xls;
using ClosedXML.Excel;
namespace DatabasePratice {
    internal class Program
    {
        static void Main(string[] args)
        {
           Update1 update1 = new Update1();
            update1.UpdateDetailName();



        }

    }
};


