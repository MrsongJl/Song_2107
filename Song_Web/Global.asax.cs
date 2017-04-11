using Song_Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Song_Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //定时任务处理 
            //调用实例 [AutoTask(EnterMethod = "StartTask", IntervalSeconds = 600, StartTime = "2016-12-28 10:45:00")]
            //调用说明
            //AutoTaskAttribute.RegisterTask();
        }
        /// <summary>
        /// 测试任务，每10分钟执行一次
        /// </summary>
        //[AutoTask(EnterMethod = "StartTask", IntervalSeconds = 600, StartTime = "2016-12-28 10:45:00")]
        //public class TestTask
        //{
        //    /// <summary>
        //    ////// </summary>
        //    public static void StartTask()
        //    {
        //        LogHelper.Info("定时任务启动成功！");
        //        LogHelper.Info("定时任务执行完毕！");
        //    }
        //}

    }
}
