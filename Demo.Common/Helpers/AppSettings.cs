using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Demo.Common
{
    /// <summary>
    /// appsettings.json 操作类（全局静态配置入口）
    /// </summary>
    public class AppSettings
    {
        public static IConfiguration Configuration { get; set; }

        static AppSettings()
        {
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = true, ReloadOnChange = true })
                .Build();
        }

        public AppSettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 读取指定 json 文件的某个节点
        /// </summary>
        public static string GetJson(string jsonPath, string key)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(jsonPath, optional: true, reloadOnChange: true).Build();
            return config.GetSection(key).Value;
        }

        /// <summary>
        /// 绑定配置到对象
        /// </summary>
        public static T Bind<T>(string key, T t)
        {
            Configuration.Bind(key, t);
            return t;
        }
    }
}
