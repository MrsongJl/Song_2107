using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ToTypeHelper
{
    /// <summary>
    /// 转int
    /// </summary>
    public static int Toint(this IEnumerable<string> items)
    {
        return Convert.ToInt32(items);
    }



}
