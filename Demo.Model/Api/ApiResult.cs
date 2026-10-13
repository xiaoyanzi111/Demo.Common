using System;
using System.Collections.Generic;

namespace Demo.Model.Api
{
    /// <summary>
    /// 统一接口返回（无数据）
    /// </summary>
    public class ApiResult
    {
        public ApiResult()
        {
            Status = 0;
        }

        /// <summary>
        /// 请求状态码
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回时间戳（秒）
        /// </summary>
        public string Timestamp { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
    }

    /// <summary>
    /// 统一接口返回（带数据）
    /// </summary>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 接口返回值
        /// </summary>
        public T Result { get; set; }
    }
}
