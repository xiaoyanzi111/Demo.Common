using SqlSugar;

namespace Demo.Model.Entity
{
    /// <summary>
    /// 示例实体（仅用于演示分层调用链，可直接删除）
    /// 对应数据表：SAMPLE_TABLE
    /// </summary>
    [SugarTable("SAMPLE_TABLE")]
    public class SampleEntity : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "NAME", Length = 100, IsNullable = true)]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "REMARK", Length = 500, IsNullable = true)]
        public string Remark { get; set; }
    }
}
