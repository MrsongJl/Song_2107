using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Song_Public
{
    /// <summary>
    /// 定时Task 任务设定需要在同一个dll中 否则会找不到
    /// 暂时以Task形式
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoTaskAttribute : Attribute
    {
        /// <summary>
        /// 入口程序
        /// </summary>
        public string EnterMethod { get; set; }
        /// <summary>
        /// 执行间隔秒数（未设置或0 则只执行一次）
        /// </summary>
        public int IntervalSeconds { get; set; }
        /// <summary>
        /// 开始执行日期
        /// </summary>
        public string StartTime { get; set; }

        //保留对Timer 的引用，避免回收
        private static Dictionary<AutoTaskAttribute, System.Threading.Timer> timers = new Dictionary<AutoTaskAttribute, System.Threading.Timer>();
        /// <summary>
        /// Global.asax.cs 中调用
        /// </summary>
        public static void RegisterTask()
        {
            new Task(() => StartAutoTask()).Start();
        }
        /// <summary>
        /// 启动定时任务
        /// </summary>
        private static void StartAutoTask()
        {
            var types = Assembly.GetExecutingAssembly().ExportedTypes.Where(t => Attribute.IsDefined(t, typeof(AutoTaskAttribute))).ToList();
            foreach (var t in types)
            {
                try
                {
                    var att = (AutoTaskAttribute)Attribute.GetCustomAttribute(t, typeof(AutoTaskAttribute));
                    if (att != null)
                    {
                        if (string.IsNullOrWhiteSpace(att.EnterMethod))
                        {
                            throw new Exception("未指定任务入口！EnterMethod");
                        }
                        var ins = Activator.CreateInstance(t);
                        var method = t.GetMethod(att.EnterMethod);

                        if (att.IntervalSeconds > 0)
                        {
                            int duetime = 0; //计算延时时间

                            if (string.IsNullOrWhiteSpace(att.StartTime))
                            {
                                duetime = 1000;
                            }
                            else
                            {
                                var datetime = DateTime.Parse(att.StartTime);
                                if (DateTime.Now <= datetime)
                                {
                                    duetime = (int)(datetime - DateTime.Now).TotalSeconds * 1000;
                                }
                                else
                                {
                                    duetime = att.IntervalSeconds * 1000 - ((int)(DateTime.Now - datetime).TotalMilliseconds) % (att.IntervalSeconds * 1000);
                                }
                            }

                            timers.Add(att, new System.Threading.Timer((o) =>
                            {
                                method.Invoke(ins, null);
                            }, ins, duetime, att.IntervalSeconds * 1000));
                        }
                        else
                        {
                            method.Invoke(ins, null);
                        }
                    }

                }
                catch (Exception ex)
                {
                    //LogHelper.Error(t.FullName + " 任务启动失败", ex);
                }
            }
        }



    }
    /// <summary>
    /// 测试任务，每10分钟执行一次 任务设定需要在同一个dll中 否则会找不到
    /// </summary>
    // [AutoTask(EnterMethod = "StartTask", IntervalSeconds = 600, StartTime = "2017-04-21 16:54:00")]
    //public class TestTask
    //{
    //    /// <summary>
    //    ////// </summary>
    //    public static void StartTask()
    //    {
    //        HttpHelper a = new HttpHelper();
    //        a.GetWebRequest("http://localhost:3829/");
    //    }
    //}
}