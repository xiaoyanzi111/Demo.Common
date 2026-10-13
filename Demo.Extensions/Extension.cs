using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Demo.Extensions
{
    /// <summary>
    /// 通用对象扩展
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// 计算文件大小（保留两位小数），Size 为字节数
        /// </summary>
        public static string CountSize(long Size)
        {
            string m_strSize = "";
            long FactSize = Size;
            if (FactSize < 1024.00)
                m_strSize = FactSize.ToString("F2") + " Byte";
            else if (FactSize >= 1024.00 && FactSize < 1048576)
                m_strSize = (FactSize / 1024.00).ToString("F2") + " K";
            else if (FactSize >= 1048576 && FactSize < 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " M";
            else if (FactSize >= 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            return m_strSize;
        }

        /// <summary>
        /// 对象序列化为 JSON
        /// </summary>
        public static string ToJson(this object value) => JsonConvert.SerializeObject(value);

        /// <summary>
        /// JSON 反序列化
        /// </summary>
        public static T ToObject<T>(this string json) => JsonConvert.DeserializeObject<T>(json);

        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection == null || collection.Count == 0;
    }
}
