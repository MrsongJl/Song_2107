using Song_Public;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Song_Single
{
    public class TestHelper
    {
        // public static string str = "metadata=res://*/SongEntity.csdl|res://*/SongEntity.ssdl|res://*/SongEntity.msl;provider=System.Data.SqlClient;provider connection string=\"data source =.; initial catalog = Song_2017; integrated security = True; MultipleActiveResultSets=True;App=EntityFramework";//数据库链接串
        public static string str = "data source=.;initial catalog= Song_2017;persist security info=True;user id=sa;password=123;";
        public static DataTable dt = null;
        /// <summary>
        /// 保存100000数据 写五个线程  每个处理20万 
        /// </summary>
        public void start()
        {
            GetData();//获取值 静态的
            CopyTable.Insert(dt,str,"dbo.Test");
            //Thread thread1 = new Thread(new ThreadStart(F1));
            //thread1.Start();
            //Thread thread2 = new Thread(new ThreadStart(F1));
            //thread2.Start();
            //Thread thread3 = new Thread(new ThreadStart(F1));
            //thread3.Start();
            //Thread thread4 = new Thread(new ThreadStart(F1));
            //thread4.Start();
            //Thread thread5 = new Thread(new ThreadStart(F1));
            //thread5.Start();

        }
        private void GetData()
        {
            try
            {
                dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Sex", typeof(int));
                dt.Columns.Add("Age", typeof(int));
                dt.Columns.Add("nowGroup", typeof(string));
                for (int i = 0; i < 1000000; i++)
                {
                    DataRow dr = dt.NewRow(); //行
                    dr["Name"] = i.ToString();
                    dr["Sex"] = i;
                    dr["Age"] = i;
                    dr["nowGroup"] = i.ToString();
                    dt.Rows.Add(dr);
                }
            }
            finally
            {

            }
        }
    }
}
