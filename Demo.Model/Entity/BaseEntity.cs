using System;
using SqlSugar;

namespace Demo.Model.Entity
{
    /// <summary>
    /// 实体基类（示例）：统一主键与审计字段
    /// 说明：业务实体继承本类并按需添加 [SugarTable]、[SugarColumn] 等 SqlSugar 特性。
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "ID")]
        public long ID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "CREATE_TIME")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "CREATE_BY")]
        public string CreateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "UPDATE_TIME")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "UPDATE_BY")]
        public string UpdateBy { get; set; }

        /// <summary>
        /// 是否删除（逻辑删除）
        /// </summary>
        [SugarColumn(IsNullable = false, ColumnName = "IS_DELETED")]
        public bool IsDeleted { get; set; }
    }
}
