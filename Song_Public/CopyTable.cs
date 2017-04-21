using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public
{
    /// <summary>
    /// 实现大数据导入到数据表的操作 可以控制在20万数据1秒左右  万级以上使用
    /// </summary>
    public class CopyTable
    {
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="data">DataTable另行数据</param>
        /// <param name="connstr">数据库链接串</param>
        /// <param name="tablename">表的名称</param>
        /// <returns></returns>
        public static bool Insert(DataTable data, string connstr, string tablename)
        {
            try
            {
                //连接sql数据库语句  
                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
                    {
                        for (int i = 0; i < data.Columns.Count; i++)
                        {
                            sqlBulkCopy.ColumnMappings.Add(data.Columns[i].ColumnName, data.Columns[i].ColumnName);
                        }
                        sqlBulkCopy.BatchSize = 100000; //每批次传输条数 已10万位列
                        sqlBulkCopy.BulkCopyTimeout = 60;//超时之前操作完成所允许的秒数。  
                        sqlBulkCopy.DestinationTableName = tablename; //服务器上目标表的名称。
                        timer.Start();//精准计时器开始
                        sqlBulkCopy.WriteToServer(data);  //将data这个datatable中的表复制到目标表中。  
                        timer.Stop(); //精准计时器结束 
                    }
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                    Log4netProvider.Logger.Error("本次捕捉的时间为" + timer.ElapsedMilliseconds.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log4netProvider.Logger.Error("批量复制出错。" + ex.Message);
                return false;
            }

        }
    }
}
