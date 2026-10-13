using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Demo.Model.Api;
using SqlSugar;

namespace Demo.Service
{
    /// <summary>
    /// 泛型服务基类：封装最常见 CRUD / 分页 / 缓存能力
    /// 优化点：通过 IUnitOfWork 暴露的 ISqlSugarClient 访问数据库，
    /// 并补充了 Async 版本方法，方便在异步链路中直接使用。
    /// </summary>
    public class BaseService<T> : IBaseService<T> where T : class, new()
    {
        private readonly IUnitOfWork _unitOfWork;

        public ISqlSugarClient Db => _unitOfWork.Db;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region 添加操作
        public int Add(T parm) => Db.Insertable(parm).RemoveDataCache().ExecuteCommand();

        public int Add(List<T> parm) => Db.Insertable(parm).RemoveDataCache().ExecuteCommand();

        public Task<int> AddAsync(T parm) => Db.Insertable(parm).RemoveDataCache().ExecuteCommandAsync();

        public Task<int> AddAsync(List<T> parm) => Db.Insertable(parm).RemoveDataCache().ExecuteCommandAsync();
        #endregion

        #region 查询操作
        public bool Any(Expression<Func<T, bool>> where) => Db.Queryable<T>().Any(where);

        public Task<bool> AnyAsync(Expression<Func<T, bool>> where) => Db.Queryable<T>().AnyAsync(where);

        public TResult Sum<TResult>(Expression<Func<T, bool>> where, Expression<Func<T, TResult>> field)
            => Db.Queryable<T>().Where(where).Sum(field);

        public T GetId(object pkValue) => Db.Queryable<T>().InSingle(pkValue);

        public Task<T> GetIdAsync(object pkValue) => Db.Queryable<T>().InSingleAsync(pkValue);

        public List<T> GetIn(object[] ids) => Db.Queryable<T>().In(ids).ToList();

        public int GetCount(Expression<Func<T, bool>> where) => Db.Queryable<T>().Count(where);

        public Task<int> GetCountAsync(Expression<Func<T, bool>> where) => Db.Queryable<T>().CountAsync(where);

        public List<T> GetAll(bool useCache = false, int cacheSecond = 3600)
            => Db.Queryable<T>().WithCacheIF(useCache, cacheSecond).ToList();

        public T GetFirst(Expression<Func<T, bool>> where) => Db.Queryable<T>().Where(where).First();

        public Task<T> GetFirstAsync(Expression<Func<T, bool>> where) => Db.Queryable<T>().Where(where).FirstAsync();

        public PagedInfo<T> GetPages(Expression<Func<T, bool>> where, PageParm parm)
        {
            int totalCount = 0, totalPages = 0;
            var list = Db.Queryable<T>().Where(where)
                .OrderByIF(!string.IsNullOrEmpty(parm.Sort), $"{parm.OrderBy} {(parm.Sort == "descending" ? "desc" : "asc")}")
                .ToPageList(parm.PageIndex, parm.PageSize, ref totalCount, ref totalPages);

            return new PagedInfo<T>
            {
                PageSize = parm.PageSize,
                PageIndex = parm.PageIndex,
                TotalCount = totalCount,
                TotalPages = totalPages,
                DataSource = list
            };
        }

        public async Task<PagedInfo<T>> GetPagesAsync(Expression<Func<T, bool>> where, PageParm parm)
        {
            RefAsync<int> totalCount = 0;
            var list = await Db.Queryable<T>().Where(where)
                .OrderByIF(!string.IsNullOrEmpty(parm.Sort), $"{parm.OrderBy} {(parm.Sort == "descending" ? "desc" : "asc")}")
                .ToPageListAsync(parm.PageIndex, parm.PageSize, totalCount);

            return new PagedInfo<T>
            {
                PageSize = parm.PageSize,
                PageIndex = parm.PageIndex,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)parm.PageSize),
                DataSource = list
            };
        }

        public List<T> GetWhere(Expression<Func<T, bool>> where, bool useCache = false, int cacheSecond = 3600)
            => Db.Queryable<T>().Where(where).WithCacheIF(useCache, cacheSecond).ToList();

        public List<T> GetWhere(Expression<Func<T, bool>> where, Expression<Func<T, object>> order, string orderEnum = "Asc", bool useCache = false, int cacheSecond = 3600)
            => Db.Queryable<T>().Where(where)
                .OrderByIF(orderEnum == "Asc", order, OrderByType.Asc)
                .OrderByIF(orderEnum == "Desc", order, OrderByType.Desc)
                .WithCacheIF(useCache, cacheSecond).ToList();
        #endregion

        #region 修改操作
        public int Update(T parm) => Db.Updateable(parm).RemoveDataCache().ExecuteCommand();

        public int Update(T parm, Expression<Func<T, object>> columns)
            => Db.Updateable(parm).WhereColumns(columns).RemoveDataCache().ExecuteCommand();

        public int Update(List<T> parm) => Db.Updateable(parm).RemoveDataCache().ExecuteCommand();

        public int Update(List<T> parm, Expression<Func<T, object>> columns)
            => Db.Updateable(parm).WhereColumns(columns).RemoveDataCache().ExecuteCommand();

        public int Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns)
            => Db.Updateable<T>().SetColumns(columns).Where(where).RemoveDataCache().ExecuteCommand();

        public Task<int> UpdateAsync(T parm) => Db.Updateable(parm).RemoveDataCache().ExecuteCommandAsync();

        public Task<int> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns)
            => Db.Updateable<T>().SetColumns(columns).Where(where).RemoveDataCache().ExecuteCommandAsync();
        #endregion

        #region 删除操作
        public int Delete(object id) => Db.Deleteable<T>(id).RemoveDataCache().ExecuteCommand();

        public int Delete(object[] ids) => Db.Deleteable<T>().In(ids).RemoveDataCache().ExecuteCommand();

        public int Delete(Expression<Func<T, bool>> where) => Db.Deleteable<T>().Where(where).RemoveDataCache().ExecuteCommand();

        public Task<int> DeleteAsync(Expression<Func<T, bool>> where) => Db.Deleteable<T>().Where(where).RemoveDataCache().ExecuteCommandAsync();
        #endregion

        #region 添加或更新
        public T Saveable(T parm, Expression<Func<T, object>> uClumns = null, Expression<Func<T, object>> iColumns = null)
        {
            var command = Db.Saveable(parm);
            if (uClumns != null) command = command.UpdateIgnoreColumns(uClumns);
            if (iColumns != null) command = command.InsertIgnoreColumns(iColumns);
            return command.RemoveDataCache().ExecuteReturnEntity();
        }

        public List<T> Saveable(List<T> parm, Expression<Func<T, object>> uClumns = null, Expression<Func<T, object>> iColumns = null)
        {
            var command = Db.Saveable(parm);
            if (uClumns != null) command = command.UpdateIgnoreColumns(uClumns);
            if (iColumns != null) command = command.InsertIgnoreColumns(iColumns);
            return command.RemoveDataCache().ExecuteReturnList();
        }

        public StorageableResult<T> Storageable(T parm, Expression<Func<T, object>> where)
            => Db.Storageable(parm).WhereColumns(where).ToStorage();

        public StorageableResult<T> Storageable(List<T> parm, Expression<Func<T, object>> where)
            => Db.Storageable(parm).WhereColumns(where).ToStorage();
        #endregion
    }
}
