using System;
using System.ComponentModel;
using System.Reflection;

namespace Demo.Common.Constants
{
    /// <summary>
    /// 枚举扩展：读取 [Description] 文本
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获得枚举的 Description 文本
        /// </summary>
        public static string GetText(this Enum obj)
        {
            Type type = obj.GetType();
            FieldInfo field = type.GetField(obj.ToString());
            if (field == null) return obj.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? obj.ToString();
        }
    }

    /// <summary>
    /// 统一响应状态码
    /// </summary>
    public enum StatusCodeType
    {
        [Description("请求(或处理)成功")]
        Success = 200,

        [Description("内部请求出错")]
        Error = 500,

        [Description("访问请求未授权! 当前 SESSION 失效, 请重新登陆")]
        Unauthorized = 401,

        [Description("请求参数不完整或不正确")]
        ParameterError = 400,

        [Description("您无权进行此操作，请求执行已拒绝")]
        Forbidden = 403,

        [Description("找不到与请求匹配的 HTTP 资源")]
        NotFound = 404,

        [Description("HTTP请求类型不合法")]
        HttpMehtodError = 405,

        [Description("HTTP请求不合法,请求参数可能被篡改")]
        HttpRequestError = 406,

        [Description("该URL已经失效")]
        URLExpireError = 407,

        [Description("操作不成功")]
        ErrorData = 999,
    }

    /// <summary>
    /// 来源类型
    /// </summary>
    public enum SourceType
    {
        [Description("Web")]
        Web = 0,

        [Description("MiniProgram")]
        MiniProgram = 1,

        [Description("App")]
        App = 2,
    }
}
