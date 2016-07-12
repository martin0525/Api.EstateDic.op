using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}