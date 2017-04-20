using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Song_Public
{
    /// <summary>
    ///  考虑到微信授权返回页面的特殊，参数判断返回页面行不通
    /// </summary>
    public class LoginBackUrl
    {

        /// <summary>
        ///没登录时设置缓存页面
        /// </summary>
        /// <returns></returns>
        public static void Set()
        {
            var url = HttpContext.Current.Request.Url.ToString();
            //客户端cookie  HttpContext.Current.Response 请求对象  HttpContext.Current.Request
            CookieHelper.Set("backurl", url, HttpContext.Current.Response);

        }

        /// <summary>
        /// 获取登录前的页面处理
        /// </summary>
        public static string Get()
        {
            //客户端cookie  HttpContext.Current.Response 请求对象  HttpContext.Current.Request
            var url = CookieHelper.Get("backurl", HttpContext.Current.Request);
            //删除客户端缓存
            CookieHelper.Delete("backurl", HttpContext.Current.Response, HttpContext.Current.Request);
            return url;
        }
    }
}
