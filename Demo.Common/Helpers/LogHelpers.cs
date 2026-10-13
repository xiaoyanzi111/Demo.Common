using System;

namespace Demo.Common.Helpers
{
    /// <summary>
    /// 日志文本格式化助手
    /// </summary>
    public static class LogHelpers
    {
        public static string logWrite(string message)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}] => {message}";
        }
    }
}
