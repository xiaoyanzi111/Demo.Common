using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Demo.Common.Helpers
{
    /// <summary>
    /// 通用 HTTP 请求助手（基于 HttpClient，静态单例复用连接）
    /// </summary>
    public static class PublicHelper
    {
        private static readonly HttpClient _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        /// <summary>
        /// 判断字符串是否全部为数字
        /// </summary>
        public static bool IsNumber(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return Regex.IsMatch(s, "^[0-9]*$");
        }

        /// <summary>
        /// 发送 JSON POST 请求
        /// </summary>
        public static string SendJsonPost(string url, string jsonParam, string token = "", string key = "", string sign = "", string requestTime = "")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(jsonParam, Encoding.UTF8, "application/json");

                if (!string.IsNullOrEmpty(token))
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + token);
                if (!string.IsNullOrEmpty(key))
                    request.Headers.TryAddWithoutValidation("key", key);
                if (!string.IsNullOrEmpty(sign))
                    request.Headers.TryAddWithoutValidation("sign", sign);
                if (!string.IsNullOrEmpty(requestTime))
                    request.Headers.TryAddWithoutValidation("requestTime", requestTime);

                using var response = _client.Send(request);
                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 发送表单 POST 请求（application/x-www-form-urlencoded）
        /// </summary>
        public static string SendFormPost(string url, IDictionary<string, string> parameters, string authorization = "")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new FormUrlEncodedContent(parameters ?? new Dictionary<string, string>());
                if (!string.IsNullOrEmpty(authorization))
                    request.Headers.TryAddWithoutValidation("Authorization", authorization);

                using var response = _client.Send(request);
                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        public static string SendGet(string url, IDictionary<string, string> parameters = null, string authorization = "", string key = "", string sign = "", string requestTime = "")
        {
            try
            {
                if (parameters != null && parameters.Count > 0)
                    url += "?" + BuildQuery(parameters);

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (!string.IsNullOrEmpty(authorization))
                    request.Headers.TryAddWithoutValidation("Authorization", authorization);
                if (!string.IsNullOrEmpty(key))
                    request.Headers.TryAddWithoutValidation("key", key);
                if (!string.IsNullOrEmpty(sign))
                    request.Headers.TryAddWithoutValidation("sign", sign);
                if (!string.IsNullOrEmpty(requestTime))
                    request.Headers.TryAddWithoutValidation("requestTime", requestTime);

                using var response = _client.Send(request);
                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 异步发送 GET 请求
        /// </summary>
        public static async Task<string> SendGetAsync(string url, IDictionary<string, string> parameters = null, string authorization = "")
        {
            if (parameters != null && parameters.Count > 0)
                url += "?" + BuildQuery(parameters);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(authorization))
                request.Headers.TryAddWithoutValidation("Authorization", authorization);

            using var response = await _client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 组装 URL 查询串
        /// </summary>
        private static string BuildQuery(IDictionary<string, string> parameters)
        {
            var postData = new StringBuilder();
            bool hasParam = false;
            foreach (var kv in parameters)
            {
                if (string.IsNullOrEmpty(kv.Key)) continue;
                if (hasParam) postData.Append('&');
                postData.Append(kv.Key).Append('=').Append(Uri.EscapeDataString(kv.Value ?? string.Empty));
                hasParam = true;
            }
            return postData.ToString();
        }
    }
}
