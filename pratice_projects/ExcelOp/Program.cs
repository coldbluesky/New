using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Office;
using System.IO;
using ClosedXML;
using Spire.Xls;
using System.Data;
using MySql.Data.MySqlClient;

namespace ExcelOp
{
    public class Program
    {
        //1-上料输出，2上料输入，3-下料输出，4-下料输入
        static string leftBrace = "{";
        static string rightBrace = "}";
        static string comma = ",";
        static string equal = " = ";
        static string prefix = "public enum ";

        //生成enum声明
        public string GetEnumDeclare(string enumName) 
        {
            return prefix + enumName;
        }
        //生成描述
        public string GetDescription(string name)
        {
            string descriptrion = string.Format("[Description(\"{0}\")]", name);
            return descriptrion;
        }
        public DataSet GetDataSet(string sql)
        {
            string str = "server=172.30.30.4;port=3306;user=root;password=123456;database=Baking";
            MySqlConnection conn = new MySqlConnection(str);

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            //初始化dataset对象
            DataSet ds = new DataSet();
            //adapter向dataset填入数据
            adapter.Fill(ds);
            return ds;
        }

        public void Writer(string sql, string enumName)
        {
            DataSet ds = GetDataSet(sql);
            StreamWriter sw = new StreamWriter(@"D:\pratice_projects\ExcelOp\1.cs", true);
            sw.WriteLine(GetEnumDeclare(enumName));
            sw.WriteLine(leftBrace);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                sw.WriteLine("\t" + GetDescription(dr["ParamDesc"].ToString()));
                sw.WriteLine("\t" + dr["ParamName"].ToString() + equal + dr["Offset"].ToString() + comma);
                sw.WriteLine();
                sw.Flush();
            }

            sw.WriteLine(rightBrace);
            sw.Close();
        }
        public void StoveWriter(string sql, string enumName)
        {
            DataSet ds = GetDataSet(sql);
            StreamWriter sw = new StreamWriter(@"D:\pratice_projects\ExcelOp\1.cs", true);
            sw.WriteLine(GetEnumDeclare(enumName));
            sw.WriteLine(leftBrace);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                sw.WriteLine("\t" + GetDescription(dr["VarDesc"].ToString()));
                sw.WriteLine("\t" + dr["ParamName"].ToString()+ comma);
                sw.WriteLine();
                sw.Flush();
            }

            sw.WriteLine(rightBrace);
            sw.Close();
        }
        //生成上料输出
        public void GenerateLoadingOutput()
        {
            string sql = string.Format("select *from TStoveConfig where RuleId = 1");
            Writer(sql, "LoadingOutput");
           
        }

        //生成上料输入
        public void GenerateLoadingInput()
        {
            string sql = string.Format("select *from TStoveConfig where RuleId = 2");
            Writer(sql, "LoadingInput");

        }

        //生成下料输出
        public void GenerateBlankingOutput()
        {
            string sql = string.Format("select *from TStoveConfig where RuleId = 3");
            Writer(sql,"BlankingOutput");
        }

        //生成下料输入
        public void GenerateBlankingInput()
        {
            string sql = string.Format("select *from TStoveConfig where RuleId = 4");
            Writer(sql, "BlankingInput");
        }

        //生成炉子输出
        public void GenerateStoveInput()
        {
            string sql = string.Format("SELECT DISTINCT ParamName, ParamDesc FROM TStoveConfig WHERE  RuleId = 6 OR RuleId = 10 OR RuleId = 11 OR RuleId = 16 OR RuleId = 20 OR RuleId = 21");
            StoveWriter(sql, "EStoveInput");
        }
        public void GenerateStoveOutput()
        {
            string sql = string.Format("SELECT DISTINCT ParamName,ParamDesc FROM TStoveConfig WHERE RuleId=5  " +
                "OR RuleId=7 OR RuleId = 8 OR RuleId=9 OR RuleId = 15  or RuleId = 17  or RuleId = 18  or RuleId = 19 or RuleId = 30");
            StoveWriter(sql, "EStoveOutput");
        }
        //生成报警
        public void GenerateAlarm()
        {
            string sql = string.Format("select DISTINCT ParamName,ParamDesc from TStoveConfig where RuleId = 22 or RuleId = 12");
            StoveWriter(sql, "StoveAlarm");
        }

        public void G6098()
        {
            string sql = "SELECT  ParamName,  VarDesc FROM `TTagList` where MachineIds = '10' GROUP BY ParamName";
            StoveWriter(sql, "EBlankingPLC");
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            //p.GenerateLoadingOutput();
            //p.GenerateLoadingInput();
            //p.GenerateBlankingOutput();
            //p.GenerateBlankingInput();
            //p.GenerateStoveInput();
            //p.GenerateStoveOutput();
            //p.GenerateAlarm();
            p.G6098();
        }
    }
}
