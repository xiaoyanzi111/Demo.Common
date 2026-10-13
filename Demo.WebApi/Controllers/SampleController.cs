using System.Threading.Tasks;
using Demo.Common.Constants;
using Demo.Model.Api;
using Demo.Model.Entity;
using Demo.Service;
using Demo.WebApi.Filters;
using Demo.WebApi.Swagger;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebApi.Controllers
{
    /// <summary>
    /// 示例控制器：演示「Controller → Service → Repository(SqlSugar)」的完整分层调用链
    /// 需要数据库中存在 SAMPLE_TABLE 表（脚本见项目根目录 README.md）
    /// </summary>
    [ApiGroup(ApiGroupNames.Sample)]
    [Route("api/[controller]")]
    public class SampleController : ApiControllerBase
    {
        private readonly ISampleService _sampleService;

        public SampleController(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        [HttpGet("page")]
        public async Task<JsonResult> GetPage([FromQuery] PageQueryDtoWrapper query)
        {
            var page = await _sampleService.GetPageAsync(query ?? new PageQueryDtoWrapper());
            return ToResponse(page);
        }

        /// <summary>
        /// 新增（演示 [AuthorizeFilter] 授权）
        /// </summary>
        [AuthorizeFilter]
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] SampleEntity entity)
        {
            await _sampleService.CreateAsync(entity);
            return ToResponse(StatusCodeType.Success, "新增成功");
        }

        /// <summary>
        /// 按 Id 查询
        /// </summary>
        [HttpGet("{id}")]
        public JsonResult Get(long id)
        {
            var data = _sampleService.GetId(id);
            return ToResponse(data);
        }

        /// <summary>
        /// 删除（逻辑删除示例）
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(long id)
        {
            var rows = await _sampleService.UpdateAsync(x => x.ID == id, x => new SampleEntity { IsDeleted = true });
            return ToResponse(StatusCodeType.Success, $"删除成功，影响 {rows} 行");
        }
    }
}
