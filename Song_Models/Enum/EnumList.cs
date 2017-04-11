using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public.Enum
{
    /// <summary>
    /// 使用方式
    /// </summary>
    class EnumList
    {
        public enum UserRole
        {
            [Description("描述一")]
            Role1 = 1,
            [Description("描述二")]
            Role2 = 2
        }
    }
}
