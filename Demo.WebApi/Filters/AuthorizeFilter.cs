using System;
using Demo.Common.Helpers;
using Demo.Common.Constants;
using Demo.Model.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Demo.WebApi.Filters
{
    /// <summary>
    /// 自定义授权过滤器示例：校验 Authorization 头中的 JWT 并写入当前用户
    /// 用法：[AuthorizeFilter] 标注在 Controller 或 Action 上
    /// </summary>
    public class AuthorizeFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authorization = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authorization) || authorization.Split(' ').Length != 2)
            {
                context.Result = new JsonResult(new ApiResult { Status = (int)StatusCodeType.Unauthorized, Message = "请登录" });
                return;
            }

            string token = authorization.Split(' ')[1];
            var userInfo = OAuthHelper.GetUserInfo(token);

            if (userInfo == null || userInfo.userid == 0)
            {
                context.Result = new JsonResult(new ApiResult { Status = (int)StatusCodeType.Unauthorized, Message = "登录已失效，请重新登录" });
                return;
            }

            // 写入当前请求用户，后续可通过 OAuthHelper.GetUserInfo() 获取
            OAuthHelper.SetUserInfo(userInfo);
        }
    }
}
