using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Demo.Extensions
{
    /// <summary>
    /// 全局静态配置入口（在宿主启动时注入 IConfiguration）
    /// </summary>
    public class AppConfig
    {
        public static IConfiguration Configuration { get; set; }

        public static IConfigurationSection GetSection(string name)
        {
            if (Configuration == null)
                throw new Exception("AppConfig.Configuration 未初始化，请在宿主启动时赋值。");

            return Configuration.GetSection(name);
        }

        public static string GetValue(string name) => GetSection(name).Value;

        public static string GetValue(params string[] sections)
        {
            try
            {
                if (sections.Any())
                    return Configuration[string.Join(":", sections)];
            }
            catch
            {
                // ignore
            }
            return "";
        }

        public static T GetValue<T>(string name) => GetSection(name).Get<T>();

        public static List<T> GetValueList<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }

        public static string GetConnectionString(string name) => Configuration.GetConnectionString(name);
    }
}
