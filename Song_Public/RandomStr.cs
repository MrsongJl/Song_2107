using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public
{
    /// <summary>
    /// 使用实例   RandomStr.GetSerialNumber(4).ToString()
    /// </summary>
    class RandomStr
    {
        /// <summary>
        /// 获取常用随机字符串
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static long GetSerialNumber(int Number = 4)
        {
            string text = System.DateTime.Now.ToString("yyMMddHHmmss");
            text += RandomStr.GetRandNum(Number);
            return long.Parse(text);
        }
        public static int GetUID(int id)
        {
            object obj = new object();
            int result = 0;
            lock (obj)
            {
                result = 10000 + id;
            }
            return result;
        }
        public static int GetRandomNumber(int length = 6)
        {
            return Convert.ToInt32(RandomStr.GetRandNum(length));
        }
        public static string GetEmailCode()
        {
            return RandomStr.GetRandNum(8);
        }
        public static string GetRandNum(int num)
        {
            int minValue = 1000;
            int maxValue = 9999;
            switch (num)
            {
                case 1:
                case 2:
                    minValue = 10;
                    maxValue = 99;
                    break;
                case 3:
                    minValue = 100;
                    maxValue = 999;
                    break;
                case 4:
                    minValue = 1000;
                    maxValue = 9999;
                    break;
                case 5:
                    minValue = 10000;
                    maxValue = 99999;
                    break;
                case 6:
                    minValue = 100000;
                    maxValue = 999999;
                    break;
                case 7:
                    minValue = 1000000;
                    maxValue = 9999999;
                    break;
                case 8:
                    minValue = 10000000;
                    maxValue = 99999999;
                    break;
            }
            string empty = string.Empty;
            System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
            return empty + random.Next(minValue, maxValue);
        }
    }
}
