using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;
using Quartz;

namespace Demo.TaskServer
{
    /// <summary>
    /// 任务基类：统一异常捕获、耗时统计与日志输出
    /// </summary>
    public abstract class JobBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 执行任务（统一包装）
        /// </summary>
        protected async Task<string> ExecuteJob(IJobExecutionContext context, Func<Task> job)
        {
            string jobHistory = $"Run Job [Id：{context.JobDetail.Key.Name}，Group：{context.JobDetail.Key.Group}]";
            try
            {
                var stopwatch = Stopwatch.StartNew();
                await job();
                stopwatch.Stop();
                jobHistory += $"，Succeed，Elapsed：{stopwatch.Elapsed.TotalMilliseconds} ms";
                logger.Info(jobHistory);
            }
            catch (Exception ex)
            {
                jobHistory += $"，Fail，Exception：{ex.Message}";
                logger.Error(ex, jobHistory);
            }

            return jobHistory;
        }

        public static string logWrite(string message)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}] => {message}";
        }
    }
}
