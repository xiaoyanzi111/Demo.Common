using System;
using SqlSugar;

namespace Demo.Model.Entity
{
    /// <summary>
    /// 计划任务配置（对应数据表：SYS_TASKS_QZ）
    /// 说明：任务元数据表，供 TaskServer 动态加载调度（可按需调整字段）
    /// </summary>
    [SugarTable("SYS_TASKS_QZ")]
    public class ScheduledTask
    {
        /// <summary>
        /// 任务 Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", Length = 50)]
        public string ID { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(ColumnName = "NAME", IsNullable = true, Length = 200)]
        public string Name { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        [SugarColumn(ColumnName = "JOB_GROUP", IsNullable = true, Length = 100)]
        public string JobGroup { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        [SugarColumn(ColumnName = "ASSEMBLY_NAME", IsNullable = true, Length = 200)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务类名
        /// </summary>
        [SugarColumn(ColumnName = "CLASS_NAME", IsNullable = true, Length = 200)]
        public string ClassName { get; set; }

        /// <summary>
        /// Cron 表达式
        /// </summary>
        [SugarColumn(ColumnName = "CRON", IsNullable = true, Length = 100)]
        public string Cron { get; set; }

        /// <summary>
        /// 触发器类型（1=Cron，0=Simple）
        /// </summary>
        [SugarColumn(ColumnName = "TRIGGER_TYPE", IsNullable = true)]
        public int TriggerType { get; set; }

        /// <summary>
        /// 间隔秒数（Simple 触发器）
        /// </summary>
        [SugarColumn(ColumnName = "INTERVAL_SECOND", IsNullable = true)]
        public int IntervalSecond { get; set; }

        /// <summary>
        /// 运行次数（Simple 触发器，0=无限）
        /// </summary>
        [SugarColumn(ColumnName = "RUN_TIMES", IsNullable = true)]
        public int RunTimes { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn(ColumnName = "BEGIN_TIME", IsNullable = true)]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [SugarColumn(ColumnName = "END_TIME", IsNullable = true)]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 任务参数（JSON）
        /// </summary>
        [SugarColumn(ColumnName = "JOB_PARAMS", IsNullable = true, Length = 2000)]
        public string JobParams { get; set; }
    }
}
