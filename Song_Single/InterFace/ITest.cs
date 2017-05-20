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
    public  interface ITest
    {
        /// <summary>
        /// 测试大数据缓存到redis中所使用的内存空间
        /// </summary>
        /// <returns></returns>
        IList Test();
        /// <summary>
        /// 快速的筛选到一条想要的数据
        /// </summary>
        /// <returns></returns>
        IList Test1();
        /// <summary>
        /// 利用缓存+锁 有效的处理并发
        /// </summary>
        /// <returns></returns>
        void Test2();
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
                // timer.Start();//精准计时器开始
                //var first = query.Where(a => a.Name == "姓名1781");
                //timer.Stop();//精准计时器开始
                //Log4netProvider.Logger.Error("读取缓存的查找时间" + timer.ElapsedMilliseconds.ToString());
            }
            else
            {
                timer.Start();//精准计时器开始
                query = helper.getSearchList(a => a.Id < 1202000).ToList();
                timer.Stop();//精准计时器开始
                Log4netProvider.Logger.Error("读取数据库的时间为" + timer.ElapsedMilliseconds.ToString());
                //在内存中找符合条件的
                //timer.Start();//精准计时器开始
                //var first = query.Where(a => a.Name == "姓名1781");
                //timer.Stop();//精准计时器开始
                //Log4netProvider.Logger.Error("读取内存的查找时间" + timer.ElapsedMilliseconds.ToString());
                //直接写入内存会出错
                //把对象写入redis
                RedisHelper.SetStringKey<List<Test>>("Test", query);
            }
            return query;
        }


        /// <summary>
        /// 1000万左右的数据在2秒内查询完成 需注意一些查询优化 
        /// </summary>
        /// <returns></returns>
        IList ITest.Test1()
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            EFHelpler<Test> helper = new EFHelpler<Test>();
            timer.Start();//精准计时器开始
            var test = helper.getSearchList(a => a.nowGroup=="301998").ToList();
            timer.Stop();//精准计时器开始
            Log4netProvider.Logger.Error("根据条件检索的时间为" + timer.ElapsedMilliseconds.ToString());
            return test;
        }

        private static object obj = new object();
        /// <summary>
        /// 场景  抢单 A B C 同时发起抢单 
        /// </summary>
        /// <returns></returns>
        public void Test2()
        {
            EFHelpler<Test> helper = new EFHelpler<Test>();
            //未加处理的程序 并发数为进入4个成功 
            //var model = helper.Get(1502210);
            //if (model != null)
            //{
            //    //验证是否可以抢单 
            //    if (model.Sex == 301998)
            //    {
            //        //可以抢单
            //        Log4netProvider.Logger.Error("成功");
            //        model.Sex = 1;

            //        helper.Change(model);
            //    }
            //}
             

            //模拟缓存读取
            //新建订单时把当前订单加入缓存状态为1 抢单时获取   CacheHelper.SetCache("sex", 1);
            lock (obj) {
                if ((int)CacheHelper.GetCache("sex") == 1)
                {
                    //可以抢单
                    Log4netProvider.Logger.Error("成功");
                    CacheHelper.SetCache("sex", 2);
                    //在处理同步到数据库操作
                    //此处可是使用异步线程 
                    ThreadHelper m = new ThreadHelper();
                    m.Start(add, res);
                }
            }

        }
        public void add() {
            EFHelpler<Test> helper = new EFHelpler<Test>();
            //读取缓存 用完删除
            var model = helper.Get(1502210);
            model.Sex = 2;
            helper.Change(model);
        }
        void res() {

        }

    }
}
