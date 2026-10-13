using AutoMapper;
using Demo.Model.Dto;
using Demo.Model.Entity;

namespace Demo.Mapping
{
    /// <summary>
    /// AutoMapper 映射配置（示例）
    /// 说明：放在 Demo.Interfaces 程序集下，WebApi 启动时按程序集自动扫描加载。
    /// 新增映射：继续在 CreateMap 中登记，或新建继承 Profile 的类即可。
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // 示例映射（可按业务替换）
            CreateMap<SampleEntity, PageQueryDto>().ReverseMap();
        }
    }
}
