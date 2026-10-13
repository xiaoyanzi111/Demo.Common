using System;
using Demo.Common;
using Demo.Common.Constants;
using Demo.Model.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demo.WebApi.Filters
{
    /// <summary>
    /// 全局异常过滤器：统一捕获异常并返回标准响应结构
    /// </summary>
    public class GlobalExceptionsFilter : Attribute, IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionsFilter> _logger;

        public GlobalExceptionsFilter(IWebHostEnvironment env, ILogger<GlobalExceptionsFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var response = new ApiResult<string>
            {
                Status = (int)StatusCodeType.Error,
                Message = context.Exception.Message
            };

            if (_env.IsDevelopment())
                response.Result = context.Exception.StackTrace;

            // 统一返回 200，由 Status 字段表达业务状态，便于前端处理
            context.Result = new JsonResult(response) { StatusCode = (int)StatusCodeType.Success };

            _logger.LogError(context.Exception, WriteLog(response.Message, context.Exception));
        }

        private static string WriteLog(string throwMsg, Exception ex)
        {
            return $"\r\n【自定义错误】：{throwMsg} \r\n【异常类型】：{ex.GetType().Name} \r\n【异常信息】：{ex.Message} \r\n【堆栈调用】：{ex.StackTrace}\r\n";
        }
    }
}
