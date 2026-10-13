using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demo.TaskServer.HostedService
{
    /// <summary>
    /// 常驻后台服务基类：按固定周期执行作业
    /// 适合「每隔 N 分钟清理一次数据」这类轻量周期任务；
    /// 需要 Cron 表达式的复杂调度请使用 Quartz（见 ITaskSchedulerServer）。
    /// </summary>
    public abstract class TimedBackgroundServiceBase : BackgroundService
    {
        /// <summary>
        /// 执行周期（毫秒）
        /// </summary>
        protected abstract int IntervalMilliseconds { get; }

        /// <summary>
        /// 首次执行前是否等待一个周期
        /// </summary>
        protected virtual bool DelayOnFirstRun => true;

        protected readonly ILogger Logger;
        protected readonly IServiceProvider ServiceProvider;

        protected TimedBackgroundServiceBase(ILogger logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!DelayOnFirstRun)
                await RunOnceAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(IntervalMilliseconds, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                await RunOnceAsync();
            }
        }

        private async Task RunOnceAsync()
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                await ExecuteAsync(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{GetType().Name} 执行失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 单次执行逻辑（scope 内可解析 Scoped 服务）
        /// </summary>
        protected abstract Task ExecuteAsync(IServiceProvider scopedProvider);
    }
}
