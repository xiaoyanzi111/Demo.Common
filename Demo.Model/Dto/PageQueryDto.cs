using System.Collections.Generic;

namespace Demo.Model.Dto
{
    /// <summary>
    /// 通用分页查询入参（示例，可按业务扩展）
    /// </summary>
    public class PageQueryDto
    {
        /// <summary>
        /// 当前页（从 1 开始）
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 排序方式：asc / desc
        /// </summary>
        public string Sort { get; set; }
    }

    /// <summary>
    /// 下拉选项 DTO（示例）
    /// </summary>
    public class SelectItemDto
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}
