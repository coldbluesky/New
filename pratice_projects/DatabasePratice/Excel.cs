using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasePratice
{
    public class Excel
    {
        //string ePath = @"C:\Users\DELL\Desktop\1.xlsx";

        //XLWorkbook wb = new XLWorkbook(ePath);
        //IXLWorksheet ws1 = wb.Worksheet(1);
        //IXLWorksheet ws2 = wb.Worksheet(2);
        //Console.WriteLine(ws1.Cell("B2").Value);

        //for(int i = 1000; i < 1060; i++)
        //{
        //    //Console.WriteLine(ds.Tables[0].Rows[i][5]);
        //    string template = Convert.ToString(ds.Tables[0].Rows[i][3]);
        //    template = template.Replace("1#A","");
        //    string updateSql = String.Format("update test set " +
        //        "AddressRangeIndex='{0}'where ParameterDesc like'%{1}'",
        //        ds.Tables[0].Rows[i][5],template);
        //    cmd.CommandText = updateSql;
        //    try
        //    {
        //        cmd.ExecuteNonQuery();
        //        Console.WriteLine("更新成功！");
        //    }
        //    catch
        //    {
        //        Console.WriteLine("更新失败");
        //        throw;
        //    }

        //}
        //for (int i = 1000; i < 1060; i++)
        //{
        //    // 照着这个整理下1b 1c  2a 2b 2c的数据
        //    // BitParameterDetail
        //    string pn = Convert.ToString(ds.Tables[0].Rows[i][2]);
        //    pn = pn.Replace("1A","2C");
        //    string pd = Convert.ToString(ds.Tables[0].Rows[i][3]);
        //    pd = pd.Replace("1#A", "2#C");
        //    string insertSql = string.Format("insert into BitParameterDetail" +
        //           "(MainId,ParameterName,ParameterDesc,WordIndex," +
        //           "AddressRangeIndex,StoveLayerNo,Mechanism)" +
        //           "values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",2, pn, pd, 0, 0, 2, "*");
        //    cmd.CommandText = insertSql;
        //    try
        //    {
        //        cmd.ExecuteNonQuery();
        //        Console.WriteLine("插入成功！");
        //    }
        //    catch
        //    {
        //        Console.WriteLine("插入失败");
        //        throw;
        //    }

        //}              
        //conn.Close();
        //Console.ReadKey();
        //string str1 = "AAA_1A";
        //Console.WriteLine(str1.Replace("1A","1B"));
    }
}
