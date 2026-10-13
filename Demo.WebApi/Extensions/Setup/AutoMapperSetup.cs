using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.WebApi.Extensions.Setup
{
    /// <summary>
    /// AutoMapper 注册
    /// 约定：所有继承 Profile 的映射类放在 Demo.Interfaces 程序集中，自动扫描加载。
    /// </summary>
    public static class AutoMapperSetup
    {
        public static IServiceCollection AddAutoMapperSetup(this IServiceCollection services)
        {
            var assembly = Assembly.Load("Demo.Interfaces");

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assembly);
            });

            services.AddSingleton<IMapper>(config.CreateMapper());
            return services;
        }
    }
}
