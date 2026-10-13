using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Model.Api;
using Demo.Model.Entity;

namespace Demo.Service
{
    /// <summary>
    /// 示例业务服务接口（演示分层写法，可直接删除）
    /// </summary>
    public interface ISampleService : IBaseService<SampleEntity>
    {
        /// <summary>
        /// 示例：分页查询
        /// </summary>
        Task<PagedInfo<SampleEntity>> GetPageAsync(PageQueryDtoWrapper query);

        /// <summary>
        /// 示例：新增一条数据
        /// </summary>
        Task<int> CreateAsync(SampleEntity entity);
    }

    /// <summary>
    /// 示例分页入参（演示用）
    /// </summary>
    public class PageQueryDtoWrapper
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Keyword { get; set; }
    }
}
