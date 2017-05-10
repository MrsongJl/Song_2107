using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Song_Models;
using System.Collections;
using Song_Public;

namespace Song_Single
{   
    /// <summary>
    /// 接口【请使用表名定义】  
    /// 为什么要使用接口  我们使用的数据 多数来源一同一张表 避免重复书写和修改 不便于维护 请使用接口
    /// </summary>
 public   interface ITest
    {
        /// <summary>
        /// 实现返回值为string的方法接口
        /// </summary>
        /// <returns></returns>
        IList Test();
    }
  public  class Testimpl : ITest
    {
        public IList Test()
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            EFHelpler<Test> helper = new EFHelpler<Test>();
            var query=new List<Test>();
            //清空
            //RedisHelper.KeyDelete("Test");
            var redislist = RedisHelper.KeyExists("Test"); 
            if (redislist)
            {
                //测试执行时间
                timer.Start();//精准计时器开始
                query = RedisHelper.GetStringKey<List<Test>>("Test");
                timer.Stop();//精准计时器开始
                Log4netProvider.Logger.Error("读取缓存的时间为" + timer.ElapsedMilliseconds.ToString());
                //在缓存中找符合条件的
                timer.Start();//精准计时器开始
                var first = query.Where(a => a.Name == "姓名1781");
                timer.Stop();//精准计时器开始
                Log4netProvider.Logger.Error("读取缓存的查找时间" + timer.ElapsedMilliseconds.ToString());
            }
            else
            {
                timer.Start();//精准计时器开始
                query = helper.getSearchList(a => a.Id < 1202000).ToList();
                timer.Stop();//精准计时器开始
                Log4netProvider.Logger.Error("读取数据库的时间为" + timer.ElapsedMilliseconds.ToString());
                //在内存中找符合条件的
                timer.Start();//精准计时器开始
                var first = query.Where(a => a.Name == "姓名1781");
                timer.Stop();//精准计时器开始
                Log4netProvider.Logger.Error("读取内存的查找时间" + timer.ElapsedMilliseconds.ToString());
                //直接写入内存会出错
                //把对象写入redis
                RedisHelper.SetStringKey<List<Test>>("Test", query);
            }
            return query;
        }
    }

}
