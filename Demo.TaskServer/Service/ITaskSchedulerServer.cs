using System.Threading.Tasks;
using Demo.Model.Api;
using Demo.Model.Entity;

namespace Demo.TaskServer
{
    /// <summary>
    /// 计划任务调度中心接口
    /// </summary>
    public interface ITaskSchedulerServer
    {
        /// <summary>
        /// 开启调度
        /// </summary>
        Task<ApiResult<string>> StartTaskScheduleAsync();

        /// <summary>
        /// 停止调度
        /// </summary>
        Task<ApiResult<string>> StopTaskScheduleAsync();

        /// <summary>
        /// 添加任务
        /// </summary>
        Task<ApiResult<string>> AddTaskScheduleAsync(ScheduledTask tasksQz);

        /// <summary>
        /// 暂停任务
        /// </summary>
        Task<ApiResult<string>> PauseTaskScheduleAsync(ScheduledTask tasksQz);

        /// <summary>
        /// 恢复任务
        /// </summary>
        Task<ApiResult<string>> ResumeTaskScheduleAsync(ScheduledTask tasksQz);

        /// <summary>
        /// 删除任务
        /// </summary>
        Task<ApiResult<string>> DeleteTaskScheduleAsync(ScheduledTask tasksQz);
    }
}
