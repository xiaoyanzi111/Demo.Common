using System;
using CSRedis;
using Demo.Extensions;

namespace Demo.Core.Cache
{
    /// <summary>
    /// Redis 连接管理器（对应 appsettings 的 "RedisConfig" 节点）
    /// </summary>
    public class RedisServer
    {
        private static readonly object _lock = new object();
        private static CSRedisClient _cache;
        private static CSRedisClient _session;

        /// <summary>
        /// 业务缓存
        /// </summary>
        public static CSRedisClient Cache
        {
            get
            {
                if (_cache == null)
                {
                    lock (_lock)
                    {
                        if (_cache == null)
                        {
                            string conn = AppConfig.GetValue("RedisConfig", "RedisConnectionString") ?? "";
                            _cache = new CSRedisClient(conn);
                            RedisHelper.Initialization(_cache);
                        }
                    }
                }
                return _cache;
            }
        }

        /// <summary>
        /// 会话/Token 缓存（可与业务缓存使用不同的 db）
        /// </summary>
        public static CSRedisClient Session
        {
            get
            {
                if (_session == null)
                {
                    lock (_lock)
                    {
                        if (_session == null)
                        {
                            string conn = AppConfig.GetValue("RedisConfig", "RedisSessionConnectionString");
                            if (string.IsNullOrWhiteSpace(conn))
                                conn = AppConfig.GetValue("RedisConfig", "RedisConnectionString") ?? "";
                            _session = new CSRedisClient(conn);
                        }
                    }
                }
                return _session;
            }
        }
    }
}
