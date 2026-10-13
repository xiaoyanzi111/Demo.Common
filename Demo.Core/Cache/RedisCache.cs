using System;
using System.Collections.Generic;
using Demo.Extensions;
using SqlSugar;

namespace Demo.Core.Cache
{
    /// <summary>
    /// SqlSugar 二级缓存实现（基于 Redis）
    /// 在 SqlSugar 配置中通过 ConfigureExternalServices.DataInfoCacheService 挂载，
    /// 即可使用 WithCache() / RemoveDataCache() 等能力。
    /// </summary>
    public class RedisCache : ICacheService
    {
        private readonly string _prefix;

        public RedisCache()
        {
            _prefix = AppConfig.GetValue("RedisConfig", "RedisPrefixKey") ?? "Demo:SqlSugar:";
        }

        public void Add<V>(string key, V value) => RedisServer.Cache.Set(Key(key), value);

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
            => RedisServer.Cache.Set(Key(key), value, cacheDurationInSeconds);

        public bool ContainsKey<V>(string key) => RedisServer.Cache.Exists(Key(key));

        public V Get<V>(string key) => RedisServer.Cache.Get<V>(Key(key));

        public IEnumerable<string> GetAllKey<V>() => RedisServer.Cache.Keys($"{_prefix}*");

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (ContainsKey<V>(cacheKey))
                return Get<V>(cacheKey);

            var result = create();
            Add(cacheKey, result, cacheDurationInSeconds);
            return result;
        }

        public void Remove<V>(string key) => RedisServer.Cache.Del(Key(key));

        private string Key(string key) => $"{_prefix}{key}";
    }
}
