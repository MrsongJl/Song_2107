using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public
{
    public static class IsNullHelper
    {
        /// <summary>
        /// 使用泛型类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || items.IsEmpty();
        }
        public static Boolean IsEmpty<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            var isEmpty = !items.GetEnumerator().MoveNext();
            try
            {
                items.GetEnumerator().Reset();
            }
            catch (NotSupportedException)
            {
            }

            return isEmpty;
        }

    }
}
