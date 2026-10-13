using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Demo.Repository;
using Demo.Service;
using Demo.TaskServer;
using Demo.WebApi.Interceptors;
using Quartz.Spi;

namespace Demo.WebApi.Extensions.Setup
{
    /// <summary>
    /// Autofac 容器注册（ConfigureContainer 调用）
    ///
    /// 注册约定：
    /// 1. 泛型仓储 BaseRepository&lt;T&gt; → IBaseRepository&lt;T&gt;
    /// 2. 泛型服务 BaseService&lt;T&gt;    → IBaseService&lt;T&gt;
    /// 3. Demo.Interfaces 中「接口名 = I + 类名」的服务自动注册，并启用接口 AOP 拦截
    /// 4. Demo.TaskServer 中的服务同样按约定注册
    /// </summary>
    public static class AutofacModule
    {
        public static void RegisterDemoServices(this ContainerBuilder builder)
        {
            builder.RegisterType<LogAopInterceptor>().SingleInstance();
            builder.RegisterType<Demo.Common.JWT.JwtManager>().AsSelf().InstancePerDependency();

            // 泛型仓储 / 泛型服务基类
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerDependency();

            // 业务服务（Demo.Interfaces）
            RegisterByConvention(builder, Assembly.Load("Demo.Interfaces"));

            // 定时任务服务（Demo.TaskServer）
            RegisterByConvention(builder, Assembly.Load("Demo.TaskServer"));

            // 任务调度中心（单例）
            builder.RegisterType<JobFactory>().As<IJobFactory>().SingleInstance();
            builder.RegisterType<TaskSchedulerServer>().As<ITaskSchedulerServer>().SingleInstance();
        }

        private static void RegisterByConvention(ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.IsClass
                            && !t.IsAbstract
                            && !t.IsGenericTypeDefinition
                            && t.Name != "BaseService"
                            && t.Name != "BaseRepository"
                            && t.GetInterfaces().Any(i => i.Name == "I" + t.Name))
                // 仅暴露与类名匹配的接口（如 ISampleService）
                .As(t => t.GetInterfaces().Where(i => i.Name == "I" + t.Name))
                .InstancePerDependency()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LogAopInterceptor));
        }
    }
}
