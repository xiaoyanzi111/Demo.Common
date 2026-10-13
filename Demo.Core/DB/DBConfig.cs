using System.Collections.Generic;
using System.Linq;
using Demo.Extensions;

namespace Demo.Core.DB
{
    /// <summary>
    /// 数据库连接读取器
    /// </summary>
    public static class DBConfig
    {
        /// <summary>
        /// 读取所有已启用的数据库连接
        /// </summary>
        public static List<MutiDBConns> MutiInitConn()
        {
            DBOptions dBOptions = AppConfig.GetValue<DBOptions>("DBS");
            if (dBOptions == null || dBOptions.MutiDBConns == null || dBOptions.MutiDBConns.Count == 0)
                return new List<MutiDBConns>();

            return dBOptions.MutiDBConns.Where(i => i.Enabled).ToList();
        }

        /// <summary>
        /// 是否启用了连接串加密
        /// </summary>
        public static bool IsConnectionEncrypted()
        {
            DBOptions dBOptions = AppConfig.GetValue<DBOptions>("DBS");
            return dBOptions != null && dBOptions.ConnectionEncrypt;
        }
    }
}
