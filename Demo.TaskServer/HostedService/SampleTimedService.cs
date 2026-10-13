using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Demo.TaskServer.HostedService
{
    /// <summary>
    /// 示例常驻后台服务（演示写法，可直接删除）
    /// </summary>
    public class SampleTimedService : TimedBackgroundServiceBase
    {
        // 每 5 分钟执行一次
        protected override int IntervalMilliseconds => 5 * 60 * 1000;

        public SampleTimedService(ILogger<SampleTimedService> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        protected override Task ExecuteAsync(IServiceProvider scopedProvider)
        {
            // TODO: 在这里编写你的周期任务逻辑（可从 scopedProvider 解析 Scoped 服务）
            Logger.LogInformation($"{GetType().Name} 执行一次周期任务。");
            return Task.CompletedTask;
        }
    }
}
