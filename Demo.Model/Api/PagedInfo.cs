using System;
using System.Collections.Generic;

namespace Demo.Model.Api
{
    /// <summary>
    /// 分页返回实体
    /// </summary>
    /// <typeparam name="T">数据实体</typeparam>
    [Serializable]
    public class PagedInfo<T>
    {
        /// <summary>
        /// 分页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> DataSource { get; set; } = new List<T>();

        /// <summary>
        /// 附加合计字段
        /// </summary>
        public object TotalField { get; set; }
    }
}
