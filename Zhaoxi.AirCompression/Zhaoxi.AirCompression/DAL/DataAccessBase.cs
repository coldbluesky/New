using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.DAL
{
    public class DataAccessBase
    {
        // Sqlite的数据处理对象
        private SQLiteConnection conn = null;// 建立连接
        private SQLiteCommand comm = null;// 执行SQL
        private SQLiteDataAdapter adapter = null;// 填充数据列表
        private SQLiteTransaction trans = null;//事务

        /// <summary>
        /// 执行一个SQL语句，并将结果通过DataTable返回
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDatas(string sql, Dictionary<string, object> param = null)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
                if (conn == null)
                    conn = new SQLiteConnection(connStr);
                conn.Open();

                adapter = new SQLiteDataAdapter(sql, conn);

                if (param != null)
                {
                    List<SQLiteParameter> parameters = new List<SQLiteParameter>();
                    foreach (var item in param)
                    {
                        parameters.Add(
                            new SQLiteParameter(item.Key, DbType.String)
                            { Value = item.Value }
                        );
                        adapter.SelectCommand.Parameters.Add(new SQLiteParameter(item.Key, DbType.String)
                        { Value = item.Value });
                    }
                    //adapter.SelectCommand.Parameters.AddRange(parameters.ToArray());
                }
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                this.Dispose();
            }
        }
        public int SqlExecute(List<string> sqls)
        {
            int count = 0;
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
                if (conn == null)
                    conn = new SQLiteConnection(connStr);
                conn.Open();

                trans = conn.BeginTransaction();
                foreach (var sql in sqls)
                {
                    comm = new SQLiteCommand(sql, conn);
                    count += comm.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                this.Dispose();
            }

            return count;
        }



        public void Dispose()
        {
            if (trans != null)
            {
                trans.Dispose();
                trans = null;
            }
            if (adapter != null)
            {
                adapter.Dispose();
                adapter = null;
            }
            if (comm != null)
            {
                comm.Dispose();
                comm = null;
            }
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }
    }
}
