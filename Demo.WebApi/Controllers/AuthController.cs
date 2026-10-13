using System.Security.Claims;
using Demo.Common.JWT;
using Demo.Model.Api;
using Demo.WebApi.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Demo.WebApi.Controllers
{
    /// <summary>
    /// 登录认证示例（演示 JWT 签发；真实项目请接入数据库校验账号密码）
    /// </summary>
    [ApiGroup(ApiGroupNames.Login)]
    [Route("api/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly IConfiguration _configuration;

        public AuthController(JwtManager jwtManager, IConfiguration configuration)
        {
            _jwtManager = jwtManager;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取令牌（示例：任意账号密码都会返回 token，请自行替换为真实校验）
        /// </summary>
        [HttpPost("token")]
        public JsonResult Token([FromBody] LoginRequest request)
        {
            var claims = JwtManager.BuildClaims(
                userId: 1,
                account: request?.Account ?? "demo",
                name: request?.Account ?? "demo",
                roles: "admin");

            int expireMinutes = int.TryParse(_configuration["JwtSetting:ExpireMinutes"], out var m) ? m : 120;
            string token = _jwtManager.CreateJwtToken(_configuration, claims, expireMinutes);

            return ToResponse(new { token, expiresIn = expireMinutes * 60 });
        }
    }

    /// <summary>
    /// 登录入参
    /// </summary>
    public class LoginRequest
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
