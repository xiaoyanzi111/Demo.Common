using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Demo.Model.Api;
using SqlSugar;

namespace Demo.Repository
{
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        #region 查询实体, select() 用法
        Task<TResult> GetEntity<TResult>(Expression<Func<TEntity, bool>> predicate);
        Task<TResult> GetEntity<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 查询列表，TEntity 查询扩展
        Task<List<TEntity>> QueryList(string strOrderByFileds);
        Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> predicate, string strOrderByFileds);
        Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        #endregion

        #region 查询列表, select<dto>() 用法
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, bool>> predicate = null);
        Task<List<TResult>> QueryList<TResult>(string strOrderByFileds);
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, bool>> predicate, string strOrderByFileds);
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        #endregion

        #region 查询列表, select(expression) 表达式用法
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate = null);
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, string strOrderByFileds);
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate, string strOrderByFileds);
        Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        #endregion

        #region 查询列表, 分页用法
        Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null);
        Task<PagedInfo<TEntity>> QueryPageList(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null);
        Task<PagedInfo<TEntity>> QueryPageList(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<PagedInfo<TResult>> QueryPageList<TResult>(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null);
        Task<PagedInfo<TResult>> QueryPageList<TResult>(Expression<Func<TEntity, TResult>> selectExpression, Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null);
        #endregion

        #region 事务委托
        Task<DbResult<bool>> UseITenantTran(Func<Task> action);
        Task<DbResult<bool>> UseTran(Func<Task> action);
        #endregion
    }
}
