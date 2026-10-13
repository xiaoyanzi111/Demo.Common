using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

namespace Demo.Extensions
{
    /// <summary>
    /// IConfigurationBuilder 扩展：加载自定义 json 配置并同步到 AppConfig
    /// </summary>
    public static class AppConfigurationExtensions
    {
        public static IConfigurationBuilder AddAppSettingsFile(this IConfigurationBuilder builder, string path)
            => builder.AddAppSettingsFile(null, path, null, optional: false, reloadOnChange: false);

        public static IConfigurationBuilder AddAppSettingsFile(this IConfigurationBuilder builder, string path, bool optional)
            => builder.AddAppSettingsFile(null, path, null, optional, reloadOnChange: false);

        public static IConfigurationBuilder AddAppSettingsFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
            => builder.AddAppSettingsFile(null, path, null, optional, reloadOnChange);

        public static IConfigurationBuilder AddAppSettingsFile(this IConfigurationBuilder builder, string path, string basePath, bool optional, bool reloadOnChange)
            => builder.AddAppSettingsFile(null, path, basePath, optional, reloadOnChange);

        public static IConfigurationBuilder AddAppSettingsFile(this IConfigurationBuilder builder, IFileProvider provider, string path, string basePath, bool optional, bool reloadOnChange)
        {
            builder.Add(new JsonConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            });

            if (!string.IsNullOrEmpty(basePath))
                builder.SetBasePath(basePath);

            AppConfig.Configuration = builder.Build();
            return builder;
        }
    }
}
