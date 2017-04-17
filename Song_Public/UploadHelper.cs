using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Song_Public
{
    class UploadHelper
    {
        /// <summary>
        /// 文件的上传 
        /// </summary>
        /// <returns></returns>
        public static string upload()
        {
            var up = "txt,rar,zip,jpg,jpeg,gif,png,mp3,docx,doc,xlsx,xls,avi,mp4,flv,pdf";//能通过的文件类型
            byte[] file;
            string localname = "";
            string disposition = HttpContext.Current.Request.ServerVariables["HTTP_CONTENT_DISPOSITION"];

            if (disposition != null)
            {
                // HTML5上传
                file = HttpContext.Current.Request.BinaryRead(HttpContext.Current.Request.TotalBytes);
                localname = HttpContext.Current.Server.UrlDecode(Regex.Match(disposition, "filename=\"(.+?)\"").Groups[1].Value);// 读取原始文件名
            }
            else
            {
                var filecollection = HttpContext.Current.Request.Files;//获取请求的文件集合
                // var postedfile = filecollection.Get(inputname);  //获取指定Name
                var postedfile = filecollection[0];
                localname = postedfile.FileName;               // 读取原始文件名
                file = new Byte[postedfile.ContentLength];        // 初始化byte长度.
                // 转换为byte类型
                System.IO.Stream stream = postedfile.InputStream;
                stream.Read(file, 0, postedfile.ContentLength);
                stream.Close();
                filecollection = null;
            }
            var err = "";
            //验证文件长度及文件类型 后缀名是否符合要求
            if (file.Length == 0) err = "无文件";
            return JsonHelper.ToJson(new { error = 1, message = err });
            if (file.Length > 20971520) err = "文件大小超过20M字节";//20971520 为20M
            return JsonHelper.ToJson(new { error = 1, message = err });
            var extension = GetFileExt(localname);
            if (("," + up + ",").IndexOf("," + extension + ",") < 0) err = "上传文件扩展名必需为：" + up;
            return JsonHelper.ToJson(new { error = 1, message = err });
            //符合要求 
            var attach_subdir = "day_" + DateTime.Now.ToString("yyMMdd");//目录类型
            var attach_dir = "/Files/" + attach_subdir + "/";//根目录
            var random = new Random(DateTime.Now.Millisecond);
            var filename = DateTime.Now.ToString("yyyyMMddhhmmss") + random.Next(10000) + "." + extension; // 生成随机文件名+后缀
            var target = attach_dir + filename;
            CreateFolder(HttpContext.Current.Server.MapPath(attach_dir));
            var fs = new System.IO.FileStream(HttpContext.Current.Server.MapPath(target), System.IO.FileMode.Create, System.IO.FileAccess.Write);
            fs.Write(file, 0, file.Length);
            fs.Flush();
            fs.Close();
            var path = target;
            return JsonHelper.ToJson(new { error = 0, url = path });
        }


        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        /// <param name="fullPath">文件的完整路径</param>
        /// <returns></returns>
        public static string GetFileExt(string fullPath)
        {
            return fullPath != "" ? fullPath.Substring(fullPath.LastIndexOf('.') + 1).ToLower() : "";
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        public static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        }

    }
}
