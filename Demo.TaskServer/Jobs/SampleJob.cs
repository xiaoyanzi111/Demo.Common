using System.Threading.Tasks;
using Quartz;

namespace Demo.TaskServer.Jobs
{
    /// <summary>
    /// 示例任务（演示 Quartz Job 写法，可直接删除或改成你的业务任务）
    /// 注意：类名需与数据库 SYS_TASKS_QZ 表中 AssemblyName + ClassName 对应。
    /// </summary>
    [DisallowConcurrentExecution]
    public class SampleJob : JobBase, IJob
    {
        public SampleJob()
        {
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ExecuteJob(context, async () =>
            {
                // TODO: 在这里编写你的定时业务逻辑
                string param = context.JobDetail.JobDataMap.GetString("JobParam");
                await Task.CompletedTask;
            });
        }
    }
}
