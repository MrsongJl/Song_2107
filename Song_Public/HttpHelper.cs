using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Song_Public
{
    /// <summary>
    /// 使用实例    var httpHelper = new HttpHelper(); var result = httpHelper.GetWebRequest(url, encoding: encoding);
    /// </summary>
    public class HttpHelper
    {
        private static readonly string[] browerNames = new string[]
        {
            "Firefox",
            "Opera",
            "Chrome",
            "Netscape",
            "Safari",
            "Lynx",
            "Konqueror"
        };
        public string PostWebRequest(string postUrl, string paramData, System.Text.Encoding dataEncode = null)
        {
            string result = string.Empty;
            try
            {
                ServicePointManager.Expect100Continue = false;
                if (dataEncode == null)
                {
                    dataEncode = System.Text.Encoding.UTF8;
                }
                byte[] bytes = dataEncode.GetBytes(paramData);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = (long)bytes.Length;
                System.IO.Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                System.IO.StreamReader streamReader = new System.IO.StreamReader(httpWebResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = streamReader.ReadToEnd();
                streamReader.Close();
                httpWebResponse.Close();
                requestStream.Close();
            }
            catch (System.Exception ex)
            {
                //错误日志
                Log4netProvider.Logger.Error(ex.Message);
            }
            return result;
        }
        public string GetWebRequest(string url, System.Collections.Generic.Dictionary<string, string> heads = null, System.Text.Encoding encoding = null)
        {
            WebRequest webRequest = WebRequest.Create(url);
            if (heads != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<string, string> current in heads)
                {
                    webRequest.Headers.Add(current.Key, current.Value);
                }
            }
            if (encoding == null)
            {
                encoding = System.Text.Encoding.UTF8;
            }
            System.IO.Stream responseStream = webRequest.GetResponse().GetResponseStream();
            string source = new System.IO.StreamReader(responseStream, encoding).ReadToEnd();
            return new string(source.ToArray<char>());
        }
        public static string MapPath(string strPath)
        {
            string result;
            if (HttpContext.Current != null)
            {
                result = HttpContext.Current.Server.MapPath(strPath);
            }
            else
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.TrimStart(new char[]
                    {
                        '\\'
                    });
                }
                result = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
            return result;
        }
        /// <summary>
        /// 图片的获取及另存
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public string SaveImage(string url, string path)
        {
            WebClient webClient = new WebClient();
            string text = url.Substring(url.LastIndexOf('.') + 1).ToLower();
            string text2 = "day_" + System.DateTime.Now.ToString("yyMMdd");
            string text3 = path + "/" + text2 + "/";
            string str = RandomStr.GetSerialNumber(4).ToString() + ".png";
            string fileName = text3 + "/" + str;
            HttpHelper.CreateFolder(text3);
            string result;
            try
            {
                webClient.DownloadFile(url, fileName);
                result = text2 + "/" + str;
            }
            catch (System.Exception var_6_9E)
            {
                result = "error";
            }
            return result;
        }
        private static void CreateFolder(string folderPath)
        {
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }
        /// <summary>
        /// 获取浏览器类型
        /// </summary>
        /// <returns></returns>
        public static string GetClientBrower()
        {
            string text = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            string result;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.IndexOf("MSIE 6.0", System.StringComparison.Ordinal) > -1)
                {
                    result = "IE 6.0";
                    return result;
                }
                if (text.IndexOf("MSIE 7.0", System.StringComparison.Ordinal) > -1)
                {
                    result = "IE 7.0";
                    return result;
                }
                if (text.IndexOf("MSIE 8.0", System.StringComparison.Ordinal) > -1)
                {
                    result = "IE 8.0";
                    return result;
                }
                if (text.IndexOf("MSIE 9.0", System.StringComparison.Ordinal) > -1)
                {
                    result = "IE 9.0";
                    return result;
                }
                if (text.IndexOf("MSIE", System.StringComparison.Ordinal) > -1)
                {
                    result = "IE";
                    return result;
                }
                string[] array = HttpHelper.browerNames;
                for (int i = 0; i < array.Length; i++)
                {
                    string text2 = array[i];
                    if (text.Contains(text2))
                    {
                        result = text2;
                        return result;
                    }
                }
            }
            result = "Other";
            return result;
        }
        /// <summary>
        /// 获取客户端系统
        /// </summary>
        /// <returns></returns>
        public static string GetClientOS()
        {
            string text = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            string result;
            if (text == null)
            {
                result = "Other";
            }
            else
            {
                string text2;
                if (text.IndexOf("NT 6.1", System.StringComparison.Ordinal) > -1)
                {
                    text2 = "Windows 7";
                }
                else
                {
                    if (text.IndexOf("NT 6.0", System.StringComparison.Ordinal) > -1)
                    {
                        text2 = "Windows Vista";
                    }
                    else
                    {
                        if (text.IndexOf("NT 5.2", System.StringComparison.Ordinal) > -1)
                        {
                            text2 = "Windows 2003";
                        }
                        else
                        {
                            if (text.IndexOf("NT 5.1", System.StringComparison.Ordinal) > -1)
                            {
                                text2 = "Windows XP";
                            }
                            else
                            {
                                if (text.IndexOf("NT 5", System.StringComparison.Ordinal) > -1)
                                {
                                    text2 = "Windows 2000";
                                }
                                else
                                {
                                    if (text.IndexOf("NT 4.9", System.StringComparison.Ordinal) > -1)
                                    {
                                        text2 = "Windows ME";
                                    }
                                    else
                                    {
                                        if (text.IndexOf("NT 4", System.StringComparison.Ordinal) > -1)
                                        {
                                            text2 = "Windows NT4";
                                        }
                                        else
                                        {
                                            if (text.IndexOf("NT 98", System.StringComparison.Ordinal) > -1)
                                            {
                                                text2 = "Windows 98";
                                            }
                                            else
                                            {
                                                if (text.IndexOf("NT 95", System.StringComparison.Ordinal) > -1)
                                                {
                                                    text2 = "Windows 95";
                                                }
                                                else
                                                {
                                                    if (text.IndexOf("NT", System.StringComparison.Ordinal) > -1)
                                                    {
                                                        text2 = "Windows";
                                                    }
                                                    else
                                                    {
                                                        if (text.IndexOf("Mac", System.StringComparison.Ordinal) > -1)
                                                        {
                                                            text2 = "Mac";
                                                        }
                                                        else
                                                        {
                                                            if (text.IndexOf("Linux", System.StringComparison.Ordinal) > -1)
                                                            {
                                                                text2 = "Linux";
                                                            }
                                                            else
                                                            {
                                                                if (text.IndexOf("FreeBSD", System.StringComparison.Ordinal) > -1)
                                                                {
                                                                    text2 = "FreeBSD";
                                                                }
                                                                else
                                                                {
                                                                    if (text.IndexOf("SunOS", System.StringComparison.Ordinal) > -1)
                                                                    {
                                                                        text2 = "SunOS";
                                                                    }
                                                                    else
                                                                    {
                                                                        if (text.IndexOf("OS/2", System.StringComparison.Ordinal) > -1)
                                                                        {
                                                                            text2 = "OS/2";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (text.IndexOf("AIX", System.StringComparison.Ordinal) > -1)
                                                                            {
                                                                                text2 = "AIX";
                                                                            }
                                                                            else
                                                                            {
                                                                                if (text.ToLower().IndexOf("unix", System.StringComparison.Ordinal) > -1)
                                                                                {
                                                                                    text2 = "Unix";
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Regex.IsMatch(text, "(Bot|Crawl|Spider)"))
                                                                                    {
                                                                                        text2 = "Spiders";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        text2 = "Other";
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                result = text2;
            }
            return result;
        }
        public static string GetSpiderBot()
        {
            string text = string.Empty;
            string text2 = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            string result;
            if (text2 == null)
            {
                result = string.Empty;
            }
            else
            {
                text2 = text2.ToLower();
                if (text2.IndexOf("googlebot-image", System.StringComparison.Ordinal) > -1)
                {
                    text = "Googlebot-Image";
                }
                else
                {
                    if (text2.IndexOf("googlebot-mobile", System.StringComparison.Ordinal) > -1)
                    {
                        text = "Googlebot-Mobile";
                    }
                    else
                    {
                        if (text2.IndexOf("googlebot", System.StringComparison.Ordinal) > -1)
                        {
                            text = "Googlebot";
                        }
                        else
                        {
                            if (text2.IndexOf("feedfetcher-google", System.StringComparison.Ordinal) > -1)
                            {
                                text = "Feedfetcher-Google";
                            }
                            else
                            {
                                if (text2.IndexOf("mediapartners-google", System.StringComparison.Ordinal) > -1)
                                {
                                    text = "Google Adsense";
                                }
                                else
                                {
                                    if (text2.IndexOf("adsbot-google", System.StringComparison.Ordinal) > -1)
                                    {
                                        text = "Google AdWords";
                                    }
                                    else
                                    {
                                        if (text2.IndexOf("googlefriendconnect", System.StringComparison.Ordinal) > -1)
                                        {
                                            text = "GoogleFriendConnect";
                                        }
                                        else
                                        {
                                            if (text2.IndexOf("google", System.StringComparison.Ordinal) > -1)
                                            {
                                                text = "Google";
                                            }
                                            else
                                            {
                                                if (text2.IndexOf("yahoo! slurp;", System.StringComparison.Ordinal) > -1)
                                                {
                                                    text = "Yahoo! Slurp";
                                                }
                                                else
                                                {
                                                    if (text2.IndexOf("yahoo! slurp/3.0", System.StringComparison.Ordinal) > -1)
                                                    {
                                                        text = "Yahoo! Slurp/3.0";
                                                    }
                                                    else
                                                    {
                                                        if (text2.IndexOf("yahoo! slurp china", System.StringComparison.Ordinal) > -1)
                                                        {
                                                            text = "Yahoo! Slurp China";
                                                        }
                                                        else
                                                        {
                                                            if (text2.IndexOf("yahoofeedseeker/2.0", System.StringComparison.Ordinal) > -1)
                                                            {
                                                                text = "YahooFeedSeeker/2.0";
                                                            }
                                                            else
                                                            {
                                                                if (text2.IndexOf("yahoo-blogs", System.StringComparison.Ordinal) > -1)
                                                                {
                                                                    text = "Yahoo Blogs";
                                                                }
                                                                else
                                                                {
                                                                    if (text2.IndexOf("yahoo-mmcrawler", System.StringComparison.Ordinal) > -1)
                                                                    {
                                                                        text = "Yahoo Image";
                                                                    }
                                                                    else
                                                                    {
                                                                        if (text2.IndexOf("yahoo contentmatch crawler", System.StringComparison.Ordinal) > -1)
                                                                        {
                                                                            text = "Yahoo AD";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (text2.IndexOf("yahoo", System.StringComparison.Ordinal) > -1)
                                                                            {
                                                                                text = "Yahoo";
                                                                            }
                                                                            else
                                                                            {
                                                                                if (text2.IndexOf("msnbot/1.1", System.StringComparison.Ordinal) > -1)
                                                                                {
                                                                                    text = "MSNbot/1.1";
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (text2.IndexOf("msnbot/2.0b", System.StringComparison.Ordinal) > -1)
                                                                                    {
                                                                                        text = "MSNbot/2.0b";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (text2.IndexOf("msrabot/2.0/1.0", System.StringComparison.Ordinal) > -1)
                                                                                        {
                                                                                            text = "Msrabot/2.0/1.0";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (text2.IndexOf("msnbot-media/1.0", System.StringComparison.Ordinal) > -1)
                                                                                            {
                                                                                                text = "MSNbot-media/1.0";
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (text2.IndexOf("msnbot-products", System.StringComparison.Ordinal) > -1)
                                                                                                {
                                                                                                    text = "MSNBot-Products";
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (text2.IndexOf("msnbot-academic", System.StringComparison.Ordinal) > -1)
                                                                                                    {
                                                                                                        text = "MSNBot-Academic";
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (text2.IndexOf("msnbot-newsblogs", System.StringComparison.Ordinal) > -1)
                                                                                                        {
                                                                                                            text = "MSNBot-NewsBlogs";
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (text2.IndexOf("msnbot", System.StringComparison.Ordinal) > -1)
                                                                                                            {
                                                                                                                text = "MSNBot";
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (text2.IndexOf("baiduspider", System.StringComparison.Ordinal) > -1)
                                                                                                                {
                                                                                                                    text = "Baiduspider";
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (text2.IndexOf("baiducustomer", System.StringComparison.Ordinal) > -1)
                                                                                                                    {
                                                                                                                        text = "BaiduCustomer";
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (text2.IndexOf("baidu-thumbnail", System.StringComparison.Ordinal) > -1)
                                                                                                                        {
                                                                                                                            text = "Baidu-Thumbnail";
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (text2.IndexOf("baiduspider-mobile-gate", System.StringComparison.Ordinal) > -1)
                                                                                                                            {
                                                                                                                                text = "Baiduspider-Mobile-Gate";
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (text2.IndexOf("baidu-transcoder/1.0.6.0", System.StringComparison.Ordinal) > -1)
                                                                                                                                {
                                                                                                                                    text = "Baidu-Transcoder/1.0.6.0";
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (text2.IndexOf("Baidu", System.StringComparison.Ordinal) > -1)
                                                                                                                                    {
                                                                                                                                        text = "Baidu";
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (text2.IndexOf("sosospider", System.StringComparison.Ordinal) > -1)
                                                                                                                                        {
                                                                                                                                            text = "Sosospider";
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (text2.IndexOf("sosoblogspider", System.StringComparison.Ordinal) > -1)
                                                                                                                                            {
                                                                                                                                                text = "SosoBlogspider";
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (text2.IndexOf("sosoimagespider", System.StringComparison.Ordinal) > -1)
                                                                                                                                                {
                                                                                                                                                    text = "SosoImagespider";
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (text2.IndexOf("soso", System.StringComparison.Ordinal) > -1)
                                                                                                                                                    {
                                                                                                                                                        text = "Soso";
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (text2.IndexOf("youdaobot/1.0", System.StringComparison.Ordinal) > -1)
                                                                                                                                                        {
                                                                                                                                                            text = "YoudaoBot/1.0";
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (text2.IndexOf("yodaobot-image/1.0", System.StringComparison.Ordinal) > -1)
                                                                                                                                                            {
                                                                                                                                                                text = "YodaoBot-Image/1.0";
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (text2.IndexOf("yodaobot-reader/1.0", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                {
                                                                                                                                                                    text = "Yodaobot-Reader/1.0";
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (text2.IndexOf("youdao", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                    {
                                                                                                                                                                        text = "Youdao";
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (text2.IndexOf("sogou", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                        {
                                                                                                                                                                            text = "Sogou";
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (text2.IndexOf("ia_archiver", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                            {
                                                                                                                                                                                text = "Alexa Ia_archiver";
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (text2.IndexOf("iaarchiver", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                                {
                                                                                                                                                                                    text = "Alexa Iaarchiver";
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (text2.IndexOf("twiceler-0.9", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                                    {
                                                                                                                                                                                        text = "Twiceler-0.9";
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (text2.IndexOf("qihoo", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                                        {
                                                                                                                                                                                            text = "Qihoo";
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (text2.IndexOf("ask jeeves/teoma", System.StringComparison.Ordinal) > -1)
                                                                                                                                                                                            {
                                                                                                                                                                                                text = "Ask Jeeves/Teoma";
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (Regex.IsMatch(text2, "(Bot|Crawl|Spider)"))
                                                                                                                                                                                                {
                                                                                                                                                                                                    text = "Other Spider";
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                result = text;
            }
            return result;
        }
        public static string GetLangage()
        {
            string text = HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"];
            string result;
            if (!string.IsNullOrEmpty(text))
            {
                text = text.ToLower();
                if (text.IndexOf(',') > -1)
                {
                    result = text.Substring(0, text.IndexOf(','));
                }
                else
                {
                    if (text.IndexOf(';') > -1)
                    {
                        result = text.Substring(0, text.IndexOf(';'));
                    }
                    else
                    {
                        result = text;
                    }
                }
            }
            else
            {
                result = "";
            }
            return result;
        }
        public static string GetClientIPdata()
        {
            string text = HttpHelper.ClientIP();
            string[] array = text.Split(new char[]
            {
                '.'
            });
            double value = System.Convert.ToDouble(array[0]) * 16777216.0 + System.Convert.ToDouble(array[1]) * 65536.0 + System.Convert.ToDouble(array[2]) * 256.0 + System.Convert.ToDouble(array[3]);
            return System.Convert.ToString(value);
        }
        public static string ClientIP()
        {
            string text = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string result;
            if (!string.IsNullOrEmpty(text))
            {
                if (text.IndexOf(".", System.StringComparison.Ordinal) == -1)
                {
                    text = null;
                }
                else
                {
                    if (text.IndexOf(",", System.StringComparison.Ordinal) != -1)
                    {
                        text = text.Replace(" ", "").Replace("'", "");
                        string[] array = text.Split(",;".ToCharArray());
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (HttpHelper.IsIPAddress(array[i]) && array[i].Substring(0, 3) != "10." && array[i].Substring(0, 7) != "192.168" && array[i].Substring(0, 7) != "172.16.")
                            {
                                result = array[i];
                                return result;
                            }
                        }
                    }
                    else
                    {
                        if (HttpHelper.IsIPAddress(text))
                        {
                            result = text;
                            return result;
                        }
                        text = null;
                    }
                }
            }
            string arg_1A0_0 = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != string.Empty) ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(text))
            {
                text = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(text))
            {
                text = HttpContext.Current.Request.UserHostAddress;
            }
            result = text;
            return result;
        }
        public static bool IsIPAddress(string str1)
        {
            bool result;
            if (string.IsNullOrEmpty(str1) || str1.Length < 7 || str1.Length > 15)
            {
                result = false;
            }
            else
            {
                Regex regex = new Regex("^\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}$", RegexOptions.IgnoreCase);
                result = regex.IsMatch(str1);
            }
            return result;
        }
    }
}
