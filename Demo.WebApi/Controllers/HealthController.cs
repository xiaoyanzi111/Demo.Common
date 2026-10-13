using System;
using System.Collections.Generic;
using Demo.WebApi.Swagger;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebApi.Controllers
{
    /// <summary>
    /// 健康检查（无需数据库即可访问，用于验证框架是否正常）
    /// </summary>
    [ApiGroup(ApiGroupNames.Test)]
    [Route("api/[controller]")]
    public class HealthController : ApiControllerBase
    {
        /// <summary>
        /// 健康检查
        /// </summary>
        [HttpGet]
        public JsonResult Get()
        {
            return ToResponse(new
            {
                service = "Demo.WebApi",
                version = "1.0.0",
                serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                environment = Environment.OSVersion.ToString()
            });
        }

        /// <summary>
        /// 演示：统一返回封装
        /// </summary>
        [HttpGet("list")]
        public JsonResult List()
        {
            var list = new List<object>
            {
                new { id = 1, name = "示例数据 A" },
                new { id = 2, name = "示例数据 B" }
            };
            return ToResponse(list);
        }
    }
}
