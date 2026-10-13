using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Demo.Model.Api;
using SqlSugar;

namespace Demo.Service
{
    /// <summary>
    /// 泛型服务基类契约
    /// </summary>
    public interface IBaseService<T> where T : class, new()
    {
        #region 添加操作
        int Add(T parm);
        int Add(List<T> parm);
        Task<int> AddAsync(T parm);
        Task<int> AddAsync(List<T> parm);
        #endregion

        #region 查询操作
        bool Any(Expression<Func<T, bool>> where);
        Task<bool> AnyAsync(Expression<Func<T, bool>> where);
        TResult Sum<TResult>(Expression<Func<T, bool>> where, Expression<Func<T, TResult>> field);
        T GetId(object pkValue);
        Task<T> GetIdAsync(object pkValue);
        List<T> GetIn(object[] ids);
        int GetCount(Expression<Func<T, bool>> where);
        Task<int> GetCountAsync(Expression<Func<T, bool>> where);
        List<T> GetAll(bool useCache = false, int cacheSecond = 3600);
        T GetFirst(Expression<Func<T, bool>> where);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> where);
        PagedInfo<T> GetPages(Expression<Func<T, bool>> where, PageParm parm);
        Task<PagedInfo<T>> GetPagesAsync(Expression<Func<T, bool>> where, PageParm parm);
        List<T> GetWhere(Expression<Func<T, bool>> where, bool useCache = false, int cacheSecond = 3600);
        List<T> GetWhere(Expression<Func<T, bool>> where, Expression<Func<T, object>> order, string orderEnum = "Asc", bool useCache = false, int cacheSecond = 3600);
        #endregion

        #region 修改操作
        int Update(T parm);
        int Update(T parm, Expression<Func<T, object>> columns);
        int Update(List<T> parm);
        int Update(List<T> parm, Expression<Func<T, object>> columns);
        int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns);
        Task<int> UpdateAsync(T parm);
        Task<int> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns);
        #endregion

        #region 删除操作
        int Delete(object id);
        int Delete(object[] ids);
        int Delete(Expression<Func<T, bool>> where);
        Task<int> DeleteAsync(Expression<Func<T, bool>> where);
        #endregion

        #region 添加或更新
        T Saveable(T parm, Expression<Func<T, object>> uClumns = null, Expression<Func<T, object>> iColumns = null);
        List<T> Saveable(List<T> parm, Expression<Func<T, object>> uClumns = null, Expression<Func<T, object>> iColumns = null);
        StorageableResult<T> Storageable(T parm, Expression<Func<T, object>> where);
        StorageableResult<T> Storageable(List<T> parm, Expression<Func<T, object>> where);
        #endregion
    }
}
