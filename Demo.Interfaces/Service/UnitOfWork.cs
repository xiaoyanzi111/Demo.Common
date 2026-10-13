using System;
using SqlSugar;

namespace Demo.Service
{
    /// <summary>
    /// 工作单元实现（优化点：直接持有 ISqlSugarClient，
    /// 兼容 SqlSugarScope，避免原实现中 as SqlSugarClient 的强制转换问题）
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        public ISqlSugarClient Db { get; }

        public UnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            Db = sqlSugarClient;
        }

        public void BeginTran() => Db.Ado.BeginTran();

        public void CommitTran() => Db.Ado.CommitTran();

        public void RollbackTran() => Db.Ado.RollbackTran();
    }
}
