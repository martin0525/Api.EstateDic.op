using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;


namespace EstateDic.Helper
{
    public static class JsonExtension
    {
        public static string ToJson(this object obj)
        {
            var jsonSerializer = new JavaScriptSerializer();
            jsonSerializer.RecursionLimit = 10;
            return jsonSerializer.Serialize(obj);
        }
        public static T JsonToObject<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(T);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Deserialize<T>(str);
        }

        public static T ToJsonObject<T>(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static string ListToString(this IEnumerable<string> list, string separate = ",", string defalutString = "-")
        {
            var enumerable = list.ToArray();
            if (!enumerable.Any())
            {
                return defalutString;
            }
            return string.Join(separate, enumerable);
        }
    }
}