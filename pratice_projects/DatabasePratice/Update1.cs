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
using System.IO;
using System.Reflection;

namespace DatabasePratice
{
    public class Update1
    {
        public void UpdateDetailName()
        {
            string str = "server=172.26.12.249;port=3306;user=root;password=123456;database=JL6063";
            string sql = "select * from tmachinedetail";
            MySqlConnection conn = new MySqlConnection(str);

            //string sql1 = String.Format("select * from StoveActuaPatrol where date_format(Createtime,'%Y-%m-%d')='{0}'", "2022-10-18");

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;

            conn.Open();

            //初始化adapter对象,并给他sql和连接对象
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            //初始化dataset对象
            DataSet ds = new DataSet();
            //adapter向dataset填入数据
            adapter.Fill(ds);
            if (ds.Tables[0].Rows.Count == 0)
            {
                Console.WriteLine("数据集没有任何数据");
            }
            else
            {
                foreach(DataRow i in ds.Tables[0].Rows)
                {
                    string name = Format(i["DetailName"].ToString());
                    string sql1 = string.Format("update TMachineDetail set DetailName='{0}' where Id='{1}'", name, i["Id"].ToString());
                    cmd.CommandText = sql1;
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public string Format(string detailName)
        {
            if(detailName.Length == 12)
            {
                StringBuilder stringBuilder = new StringBuilder(detailName);

                char temp = 'a';
                temp = stringBuilder[10];
                stringBuilder[10] = stringBuilder[11];
                stringBuilder[11] = temp;









                return stringBuilder.ToString();
            }
            else
            {
                return detailName;
            }
            
        }
       
    }
}
