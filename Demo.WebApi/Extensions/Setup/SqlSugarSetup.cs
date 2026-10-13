using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Common.Security;
using Demo.Core.Cache;
using Demo.Core.DB;
using Demo.Extensions;
using Demo.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Demo.WebApi.Extensions.Setup
{
    /// <summary>
    /// SqlSugar 注册（SQL Server 为主库，支持多库/多租户与 Redis 二级缓存）
    /// </summary>
    public static class SqlSugarSetup
    {
        public static IServiceCollection AddSqlSugarSetup(this IServiceCollection services, IConfiguration configuration)
        {
            var allCon = DBConfig.MutiInitConn();
            bool encrypted = DBConfig.IsConnectionEncrypted();
            bool redisEnabled = !string.Equals(AppConfig.GetValue("RedisConfig", "Enabled"), "false", StringComparison.OrdinalIgnoreCase);

            var listConfig = new List<ConnectionConfig>();
            foreach (var q in allCon)
            {
                listConfig.Add(new ConnectionConfig
                {
                    ConfigId = q.ConnId,
                    DbType = (DbType)q.DbType,
                    ConnectionString = encrypted ? DesCbcUtil.Decrypt(q.Connection) : q.Connection,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute,
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
                        // 二级缓存（可开关）
                        DataInfoCacheService = redisEnabled ? new RedisCache() : null
                    },
                    MoreSettings = new ConnMoreSettings
                    {
                        IsAutoRemoveDataCache = true
                    }
                });
            }

            // 使用 SqlSugarScope（线程安全，多库/多租户场景推荐）
            var sqlSugar = new SqlSugarScope(listConfig, db =>
            {
                foreach (var q in allCon)
                {
                    db.GetConnection(q.ConnId).Aop.OnLogExecuting = (sql, pars) =>
                    {
                        // 控制台输出 SQL，便于本地调试（生产环境可改为写日志）
                        Console.WriteLine($"[SQL:{q.ConnId}] {sql}");
                        if (pars != null && pars.Length > 0)
                            Console.WriteLine(string.Join(",", pars.Select(it => $"{it.ParameterName}:{it.Value}")));
                    };
                }
            });

            services.AddSingleton<ISqlSugarClient>(sqlSugar);
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
