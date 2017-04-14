using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Song_Public
{
    class GetIpHelper
    {

        /// <summary>
        /// 获取客户端安全真实的IP
        /// </summary>
        /// <returns></returns>
        public static string IP()
        {
                string ip;
                if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null &&
                    !string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                    //使用了代理并且有透传IP
                    ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                else
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                return ip;
        }
    }
}
