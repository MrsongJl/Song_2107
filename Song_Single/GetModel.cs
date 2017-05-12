using Song_Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Single
{
    /// <summary>
    /// 后期加入缓存使用
    /// </summary>

    public class GetModel
    {
        public static Song_2017Entities DB
        {
            get
            {
                return new Song_2017Entities();
            }
        }
    /// <summary>
    /// 存放缓存 todo: 暂未编写
    /// </summary>
     public static Song_2017Entities DBCache
        {
            get
            {
                return new Song_2017Entities();
            }
        }

        public static Stopwatch timerObj = new Stopwatch();
        /// <summary>
        /// 验证执行时间开始
        /// </summary>
        public static void TimeStart()
        {
            timerObj.Start();
        }
        /// <summary>
        ///  验证执行时间结束
        /// </summary>
        /// <returns></returns>
        public static TimeAs TimeEnd()
        {
            TimeAs t = new TimeAs();
            timerObj.Stop();
            //Logger.GetInstance().Debug("毫秒数 :" + timerObj.ElapsedMilliseconds);
            //Logger.GetInstance().Debug("时间数 :" + timerObj.Elapsed);
            t.Milliseconds = timerObj.ElapsedMilliseconds.ToString();
            t.Time = timerObj.Elapsed.ToString();
            timerObj.Reset();
            return t;
        }


    }

    public class TimeAs
    {
        public string roots { get; set; }
        public string Milliseconds { get; set; }
        public string Time { get; set; }
    }

}
