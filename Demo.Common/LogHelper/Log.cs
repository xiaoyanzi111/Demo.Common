using System;
using NLog;

namespace Demo.Common
{
    /// <summary>
    /// 日志静态门面（底层 NLog）
    /// </summary>
    public class Log
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Info(string message) => logger.Info(message);

        public static void Debug(string message) => logger.Debug(message);

        public static void Warn(string message) => logger.Warn(message);

        public static void Error(string message) => logger.Error(message);

        public static void Error(Exception ex, string message) => logger.Error(ex, message);

        public static void Fatal(Exception ex, string message) => logger.Fatal(ex, message);
    }
}
