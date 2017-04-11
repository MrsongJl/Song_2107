using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public
{
    /// <summary>
    /// 创建一个任务 按异步执行
    /// </summary>
    class AddTask
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<Stream> GetResponseContentAsync(string url)
        {
            var httpClient = new System.Net.Http.HttpClient();
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                return null;//error
            }
        }

    }
}
