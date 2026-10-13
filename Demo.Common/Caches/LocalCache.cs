using System;
using System.Collections.Concurrent;

namespace Demo.Common.Caches
{
    /// <summary>
    /// 进程内本地缓存（简单字典，适合存放变化不频繁的配置类数据）
    /// </summary>
    public static class LocalCache
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

        public static void Set(string key, object value) => _cache[key] = value;

        public static T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out var value) && value is T t ? t : default;
        }

        public static bool Contains(string key) => _cache.ContainsKey(key);

        public static void Remove(string key) => _cache.TryRemove(key, out _);

        public static void Clear() => _cache.Clear();
    }
}
