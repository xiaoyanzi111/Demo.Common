namespace Demo.Model.Api
{
    /// <summary>
    /// 分页请求参数
    /// </summary>
    public class PageParm
    {
        private int _pageIndex = 1;
        private int _pageSize = 20;

        /// <summary>
        /// 当前页（从 1 开始）
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex < 1 ? 1 : _pageIndex;
            set => _pageIndex = value;
        }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize
        {
            get => _pageSize < 1 ? 20 : _pageSize;
            set => _pageSize = value;
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 排序方式（asc / desc）
        /// </summary>
        public string Sort { get; set; }
    }
}
