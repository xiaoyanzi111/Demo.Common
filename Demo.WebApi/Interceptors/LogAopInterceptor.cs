using System.Diagnostics;
using Castle.DynamicProxy;

namespace Demo.WebApi.Interceptors
{
    /// <summary>
    /// AOP 日志拦截器示例（Autofac.Extras.DynamicProxy / Castle）
    /// 在 Demo.Interfaces 中按约定注册的服务会自动应用本拦截器。
    /// </summary>
    public class LogAopInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                stopwatch.Stop();
                var typeName = invocation.Method.DeclaringType?.Name;
                Demo.Common.Log.Debug($"AOP => {typeName}.{invocation.Method.Name} 耗时 {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
