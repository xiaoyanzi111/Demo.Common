using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.WebApi.Extensions.Setup
{
    /// <summary>
    /// 跨域注册（来源从 appsettings.json 的 Startup:AllowOrigins 读取，用 | 分隔）
    /// </summary>
    public static class CorsSetup
    {
        public const string PolicyName = "LimitRequests";

        public static IServiceCollection AddCorsSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var origins = configuration["Startup:AllowOrigins"]?
                .Split('|', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            services.AddCors(c =>
            {
                c.AddPolicy(PolicyName, policy =>
                {
                    if (origins.Length > 0)
                        policy.WithOrigins(origins);

                    policy.AllowAnyHeader().AllowAnyMethod();

                    if (origins.Length > 0)
                        policy.AllowCredentials();
                });
            });

            return services;
        }
    }
}
