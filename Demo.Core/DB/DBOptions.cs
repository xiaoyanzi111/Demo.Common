using System.Collections.Generic;

namespace Demo.Core.DB
{
    /// <summary>
    /// 数据库连接配置（对应 appsettings.json 的 "DBS" 节点）
    /// </summary>
    public class DBOptions
    {
        /// <summary>
        /// 主库 ConnId
        /// </summary>
        public string MainDB { get; set; }

        /// <summary>
        /// 是否启用多库
        /// </summary>
        public bool MutiDBEnabled { get; set; }

        /// <summary>
        /// 连接串是否经过 DES 加密
        /// </summary>
        public bool ConnectionEncrypt { get; set; } = false;

        /// <summary>
        /// 连接列表
        /// </summary>
        public List<MutiDBConns> MutiDBConns { get; set; } = new List<MutiDBConns>();
    }

    /// <summary>
    /// 单个数据库连接配置
    /// </summary>
    public class MutiDBConns
    {
        /// <summary>
        /// 连接启用开关
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 连接 ID（多租户场景下与实体 [Tenant] 特性对应）
        /// </summary>
        public string ConnId { get; set; }

        /// <summary>
        /// 从库执行级别，越大越先执行
        /// </summary>
        public int HitRate { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get; set; } = DataBaseType.SqlServer;
    }

    /// <summary>
    /// 数据库类型（与 SqlSugar DbType 枚举顺序保持一致）
    /// </summary>
    public enum DataBaseType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4,
        Dm = 5,
        Kdbndp = 6,
    }
}
