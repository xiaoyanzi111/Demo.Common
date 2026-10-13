using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using Demo.Model.Api;
using Demo.Model.Entity;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Demo.TaskServer
{
    /// <summary>
    /// 计划任务调度中心（Quartz）
    /// </summary>
    public class TaskSchedulerServer : ITaskSchedulerServer
    {
        private Task<IScheduler> _scheduler;
        private readonly IJobFactory _jobFactory;

        public TaskSchedulerServer(IJobFactory jobFactory)
        {
            _scheduler = GetTaskSchedulerAsync();
            _jobFactory = jobFactory;
        }

        private Task<IScheduler> GetTaskSchedulerAsync()
        {
            if (_scheduler != null) return _scheduler;

            var collection = new NameValueCollection { { "quartz.serializer.type", "binary" } };
            var factory = new StdSchedulerFactory(collection);
            return _scheduler = factory.GetScheduler();
        }

        public async Task<ApiResult<string>> StartTaskScheduleAsync()
        {
            _scheduler.Result.JobFactory = _jobFactory;
            if (_scheduler.Result.IsStarted)
                return new ApiResult<string> { Status = 500, Message = "计划任务已经开启" };

            await _scheduler.Result.Start();
            return new ApiResult<string> { Status = 200, Message = "计划任务开启成功" };
        }

        public async Task<ApiResult<string>> StopTaskScheduleAsync()
        {
            if (_scheduler.Result.IsShutdown)
                return new ApiResult<string> { Status = 500, Message = "计划任务已经停止" };

            await _scheduler.Result.Shutdown();
            return new ApiResult<string> { Status = 200, Message = "计划任务已经停止" };
        }

        public async Task<ApiResult<string>> AddTaskScheduleAsync(ScheduledTask tasksQz)
        {
            var jobKey = new JobKey(tasksQz.ID, tasksQz.JobGroup);
            if (await _scheduler.Result.CheckExists(jobKey))
                return new ApiResult<string> { Status = 500, Message = $"该计划任务已经在执行:【{tasksQz.Name}】,请勿重复添加！" };

            tasksQz.BeginTime ??= DateTime.Now;
            tasksQz.EndTime ??= DateTime.MaxValue.AddDays(-1);

            var assembly = Assembly.Load(new AssemblyName(tasksQz.AssemblyName));
            var jobType = assembly.GetType(tasksQz.AssemblyName + "." + tasksQz.ClassName);

            if (!_scheduler.Result.IsStarted)
                await StartTaskScheduleAsync();

            IJobDetail job = new JobDetailImpl(tasksQz.ID, tasksQz.JobGroup, jobType);
            job.JobDataMap.Add("JobParam", tasksQz.JobParams);

            ITrigger trigger = (!string.IsNullOrWhiteSpace(tasksQz.Cron)
                                && CronExpression.IsValidExpression(tasksQz.Cron)
                                && tasksQz.TriggerType > 0)
                ? CreateCronTrigger(tasksQz)
                : CreateSimpleTrigger(tasksQz);

            await _scheduler.Result.ScheduleJob(job, trigger);
            return new ApiResult<string> { Status = 200, Message = $"启动计划任务:【{tasksQz.Name}】成功！" };
        }

        public async Task<ApiResult<string>> PauseTaskScheduleAsync(ScheduledTask tasksQz)
        {
            var jobKey = new JobKey(tasksQz.ID, tasksQz.JobGroup);
            if (!await _scheduler.Result.CheckExists(jobKey))
                return new ApiResult<string> { Status = 500, Message = $"未找计划任务:【{tasksQz.Name}】" };

            await _scheduler.Result.PauseJob(jobKey);
            return new ApiResult<string> { Status = 200, Message = $"暂停计划任务:【{tasksQz.Name}】成功" };
        }

        public async Task<ApiResult<string>> ResumeTaskScheduleAsync(ScheduledTask tasksQz)
        {
            var jobKey = new JobKey(tasksQz.ID, tasksQz.JobGroup);
            if (!await _scheduler.Result.CheckExists(jobKey))
                return new ApiResult<string> { Status = 500, Message = $"未找到计划任务:【{tasksQz.Name}】" };

            await _scheduler.Result.ResumeJob(jobKey);
            return new ApiResult<string> { Status = 200, Message = $"恢复计划任务:【{tasksQz.Name}】成功" };
        }

        public async Task<ApiResult<string>> DeleteTaskScheduleAsync(ScheduledTask tasksQz)
        {
            var jobKey = new JobKey(tasksQz.ID, tasksQz.JobGroup);
            await _scheduler.Result.DeleteJob(jobKey);
            return new ApiResult<string> { Status = 200, Message = $"删除计划任务:【{tasksQz.Name}】成功" };
        }

        #region 触发器构建
        private ITrigger CreateSimpleTrigger(ScheduledTask tasksQz)
        {
            var builder = TriggerBuilder.Create()
                .WithIdentity(tasksQz.ID, tasksQz.JobGroup)
                .StartAt(tasksQz.BeginTime.Value)
                .EndAt(tasksQz.EndTime.Value)
                .WithSimpleSchedule(x =>
                {
                    x.WithIntervalInSeconds(tasksQz.IntervalSecond);
                    if (tasksQz.RunTimes > 0) x.WithRepeatCount(tasksQz.RunTimes);
                    else x.RepeatForever();
                });

            return builder.ForJob(tasksQz.ID, tasksQz.JobGroup).Build();
        }

        private ITrigger CreateCronTrigger(ScheduledTask tasksQz)
        {
            return TriggerBuilder.Create()
                .WithIdentity(tasksQz.ID, tasksQz.JobGroup)
                .StartAt(tasksQz.BeginTime.Value)
                .EndAt(tasksQz.EndTime.Value)
                .WithCronSchedule(tasksQz.Cron)
                .ForJob(tasksQz.ID, tasksQz.JobGroup)
                .Build();
        }
        #endregion
    }
}
