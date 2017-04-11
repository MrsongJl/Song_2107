using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public
{
    class JsonHelper
    {
        public static string ToJson(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd hh:mm:ss" });
        }
        public static T JsonTo<T>(string JsonStr)
        {
            return JsonConvert.DeserializeObject<T>(JsonStr);
        }

    }
}
