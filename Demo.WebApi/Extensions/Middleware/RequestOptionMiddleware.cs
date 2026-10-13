using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Demo.WebApi.Extensions.Middleware
{
    /// <summary>
    /// 中间件：直接放行 OPTIONS 预检请求（配合 CORS 跨域）
    /// </summary>
    public class RequestOptionMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestOptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method.Equals("OPTIONS", System.StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return;
            }

            await _next.Invoke(context);
        }
    }
}
