using System;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace Demo.Common.Helpers
{
    /// <summary>
    /// Token 解析助手：从 JWT 中还原当前登录用户信息
    /// </summary>
    public static class OAuthHelper
    {
        [ThreadStatic]
        private static OAuthUser _userInfo;

        /// <summary>
        /// 获取当前请求上下文中的用户信息（配合 AuthContext 中间件使用）
        /// </summary>
        public static OAuthUser GetUserInfo() => _userInfo;

        /// <summary>
        /// 根据 token 解析用户信息
        /// </summary>
        public static OAuthUser GetUserInfo(string token)
        {
            if (_userInfo == null || !token.Equals(_userInfo.token) || _userInfo.userid == 0)
            {
                _userInfo = GetCurrentUserInfo(token);
                _userInfo.token = token;
            }
            return _userInfo;
        }

        /// <summary>
        /// 手动设置当前用户（由中间件/过滤器调用）
        /// </summary>
        public static void SetUserInfo(OAuthUser user) => _userInfo = user;

        private static OAuthUser GetCurrentUserInfo(string token)
        {
            var currentUserInfo = new OAuthUser();
            try
            {
                var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var user = JsonConvert.SerializeObject(jwtSecurityToken.Payload);
                currentUserInfo = JsonConvert.DeserializeObject<OAuthUser>(user) ?? currentUserInfo;
            }
            catch
            {
                // token 非法时返回空用户
            }
            return currentUserInfo;
        }
    }

    /// <summary>
    /// 登录用户信息载体
    /// </summary>
    public class OAuthUser
    {
        public string sub { get; set; }
        public long auth_time { get; set; }
        public string idp { get; set; }
        public long userid { get; set; }
        public string account { get; set; }
        public string name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string roles { get; set; }
        public string token { get; set; }
    }
}
