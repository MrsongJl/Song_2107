using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Song_Public
{

	/// <summary>
    /// 多数使用线程池  异步操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public  class ThreadHelper
    {
        /// <param name="taskAction">线程任务</param>
        /// <param name="callBackAction">回调方法</param>
        public void Start(Action taskAction, Action callBackAction)
        {
            //新建一个WaitCallback委托
            WaitCallback wcb = stat =>
            {
                taskAction();
                Action callback = stat as Action;
                if (callback != null)
                {
                    callback();
                }
            };
            //加入线程池,将回调的委托作为参数传入(stat其实就是callbackAction)
            ThreadPool.QueueUserWorkItem(wcb, callBackAction);
        }

    }
}
