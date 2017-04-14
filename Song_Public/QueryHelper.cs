using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Song_Public
{
    class QueryHelper
    {
        /// <summary>
        /// From表单提交的值 暂不处理文件上传 调用方式var value= QueryHelper.FromValue("");
        /// </summary>
        /// <param name="key">验证的值</param>
        /// <param name="defaultValue">默认的值</param>
        /// <param name="isEnableValidate">是否去掉html</param>
        /// <returns></returns>
        public static string FromValue(string key, string defaultValue = null, bool isEnableValidate = true)
        {

            if (HttpContext.Current.Request.Files.Count > 0)
            {
                //收到文件 暂时不处理 
                return HttpContext.Current.Request.Form[key];
            }
            var value = HttpContext.Current.Request.Form[key];
            if (value.IsNullOrEmpty())
            {
                return defaultValue;
            }
            if (isEnableValidate)
            {
                var htmlFilter = new HtmlFilter();
                return htmlFilter.filter(value);
            }

            return value;
        }

        /// <summary>
        /// Url参数获取 调用方式var value= QueryHelper.UrlValue("");
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="isEnableValidate"></param>
        /// <returns></returns>
        public static string UrlValue(string key, string defaultValue = null, bool isEnableValidate = true)
        {
            if (HttpContext.Current == null)
            {
                return null;
            }

            var v = HttpContext.Current.Request.QueryString[key];
            if (v.IsNullOrEmpty())
                return defaultValue;

            if (isEnableValidate)
            {
                var htmlFilter = new HtmlFilter();
                return htmlFilter.filter(v);
            }
            return v;
        }

        /// <summary>
        /// 文件的获取 存储 并返回相对路径  存放于本地 属于单机 调用方式var value= QueryHelper.FileValue(""); todo:还未测试 
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <returns></returns>
        public static HttpPostedFile FileValue(string key)
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                //收到文件
                try
                {
                //    var mappath = System.Web.HttpContext.Current.Server.MapPath(path);
                    var Indexes = HttpContext.Current.Request.Files.AllKeys.ToList().IndexOf(key);
                    if (Indexes != -1) {
                        var oldfile = HttpContext.Current.Request.Files[Indexes];
                        return oldfile;
                    }
                    return null;
                    //   oldfile.SaveAs(mappath);
                }
                catch (Exception e)
                {
                    ///出错返回第一个;
                    return HttpContext.Current.Request.Files[0];
                }

               
            }
            return null;
        }



        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string Decode(string s)
        {
            return HttpUtility.UrlDecode(s);
        }


        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string Encode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }



    }


}
