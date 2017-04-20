using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 
///  SmsService.NewSend(手机号, "内容");
/// </summary>
#region luosimiao-SmsSend
public class SmsHelper
{
    /// <summary>
    /// 为螺丝帽短信
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string NewSend(string mobile, string message)
    {
        var username = "api";//需替换使用
        var password = "key-f54b8566e59c749dbfeb0d8af8e3a2fc";
        var url = "https://sms-api.luosimao.com/v1/send.json";

        byte[] byteArray = Encoding.UTF8.GetBytes("mobile=" + mobile + "&message=" + message);
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
        string auth = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(username + ":" + password));
        webRequest.Headers.Add("Authorization", auth);
        webRequest.Method = "POST";
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.ContentLength = byteArray.Length;

        Stream newStream = webRequest.GetRequestStream();
        newStream.Write(byteArray, 0, byteArray.Length);
        newStream.Close();

        HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
        StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.Default);
        string result = php.ReadToEnd();

        return result;
    }

    #endregion






}
