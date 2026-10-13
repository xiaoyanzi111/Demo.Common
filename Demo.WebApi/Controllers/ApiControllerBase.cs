using System;
using Demo.Common.Constants;
using Demo.Model.Api;
using Microsoft.AspNetCore.Mvc;

namespace Demo.WebApi.Controllers
{
    /// <summary>
    /// 控制器基类：提供统一返回封装
    /// </summary>
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        #region 统一返回封装
        protected static JsonResult ToResponse(StatusCodeType statusCode)
        {
            return new JsonResult(new ApiResult
            {
                Status = (int)statusCode,
                Message = statusCode.GetText()
            });
        }

        protected static JsonResult ToResponse(StatusCodeType statusCode, string message)
        {
            return new JsonResult(new ApiResult
            {
                Status = (int)statusCode,
                Message = message
            });
        }

        protected static JsonResult ToResponse<T>(T data)
        {
            return new JsonResult(new ApiResult<T>
            {
                Status = (int)StatusCodeType.Success,
                Message = StatusCodeType.Success.GetText(),
                Result = data
            });
        }
        #endregion

        /// <summary>
        /// 生成 GUID
        /// </summary>
        protected static string GetGUID => Guid.NewGuid().ToString("N").ToUpper();
    }
}
