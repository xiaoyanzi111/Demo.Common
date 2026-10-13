using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Demo.Common.JWT
{
    /// <summary>
    /// JWT 令牌签发器
    /// </summary>
    public class JwtManager
    {
        /// <summary>
        /// 生成 JWT Token（返回带 Bearer 前缀的字符串）
        /// </summary>
        public string CreateJwtToken(IConfiguration configuration, Claim[] body, int expireMinutes = 120)
        {
            string jwtKey = configuration["JwtSetting:IssuerSigningKey"];
            string jwtIssuer = configuration["JwtSetting:ValidIssuer"];
            string jwtAudience = configuration["JwtSetting:ValidAudience"];

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: body,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return "Bearer " + new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 生成常用声明集合
        /// </summary>
        public static Claim[] BuildClaims(long userId, string account, string name, string roles)
        {
            return new List<Claim>
            {
                new Claim("userid", userId.ToString()),
                new Claim("account", account ?? string.Empty),
                new Claim("name", name ?? string.Empty),
                new Claim("roles", roles ?? string.Empty)
            }.ToArray();
        }
    }
}
