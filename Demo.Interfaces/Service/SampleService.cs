using System;
using System.Threading.Tasks;
using Demo.Common.Helpers;
using Demo.Model.Api;
using Demo.Model.Entity;

namespace Demo.Service
{
    /// <summary>
    /// 示例业务服务实现（演示分层写法，可直接删除）
    ///
    /// 写法说明：
    /// 1. 继承 BaseService&lt;T&gt;，自动获得 Add/Update/Delete/GetWhere/GetPages 等能力；
    /// 2. 构造函数注入 IUnitOfWork（以及你需要的其他服务接口）；
    /// 3. 在 Autofac 中已按「接口-实现」约定自动注册，无需手动 AddScoped。
    /// </summary>
    public class SampleService : BaseService<SampleEntity>, ISampleService
    {
        public SampleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <inheritdoc />
        public Task<PagedInfo<SampleEntity>> GetPageAsync(PageQueryDtoWrapper query)
        {
            var pageParm = new PageParm
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                OrderBy = "ID",
                Sort = "descending"
            };

            return GetPagesAsync(x => !x.IsDeleted
                && (string.IsNullOrEmpty(query.Keyword) || x.Name.Contains(query.Keyword)), pageParm);
        }

        /// <inheritdoc />
        public Task<int> CreateAsync(SampleEntity entity)
        {
            entity.CreateTime = DateTime.Now;
            entity.CreateBy = OAuthHelper.GetUserInfo()?.account ?? "system";
            return AddAsync(entity);
        }
    }
}
