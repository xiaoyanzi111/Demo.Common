using System;
using Demo.Common.Constants;
using Demo.Model.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Demo.WebApi.Filters
{
    /// <summary>
    /// 全局模型验证过滤器：模型校验不通过时统一返回参数错误
    /// </summary>
    public class GlobalActionMonitorFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var response = new ApiResult { Status = (int)StatusCodeType.ParameterError };

            foreach (var item in context.ModelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    if (!string.IsNullOrEmpty(response.Message))
                        response.Message += " | ";

                    response.Message += string.IsNullOrEmpty(error.ErrorMessage)
                        ? error.Exception?.Message
                        : error.ErrorMessage;
                }
            }

            context.Result = new JsonResult(response);
        }
    }
}
