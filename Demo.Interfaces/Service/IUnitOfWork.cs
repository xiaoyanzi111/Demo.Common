using System;
using SqlSugar;

namespace Demo.Service
{
    /// <summary>
    /// 工作单元：统一管理数据库事务
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 当前数据库客户端
        /// </summary>
        ISqlSugarClient Db { get; }

        void BeginTran();
        void CommitTran();
        void RollbackTran();
    }
}
