using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Demo.Model.Api;
using SqlSugar;

namespace Demo.Repository
{
    /// <summary>
    /// 泛型仓储基类（基于 SqlSugar）
    /// 优化点：通过构造函数注入 ISqlSugarClient（SqlSugarScope 单例），
    /// 不再依赖静态 DbScoped，便于单元测试与多租户切换。
    /// </summary>
    public class BaseRepository<TEntity> : SimpleClient<TEntity>, IBaseRepository<TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// 多租户接口
        /// </summary>
        public ITenant itenant { get; }

        public BaseRepository(ISqlSugarClient context)
        {
            // 通过实体上的 [Tenant] 特性拿到 ConfigId，实现多库自动切换
            string configId = typeof(TEntity).GetCustomAttribute<TenantAttribute>()?.configId?.ToString();
            if (!string.IsNullOrEmpty(configId) && context is ITenant tenant)
            {
                Context = tenant.GetConnection(configId);
            }
            else
            {
                Context = context;
            }

            itenant = context as ITenant;
        }

        #region 查询实体, select() 用法
        public async Task<TResult> GetEntity<TResult>(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Queryable<TEntity>().Where(predicate).Select<TResult>().FirstAsync();
        }

        public async Task<TResult> GetEntity<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Queryable<TEntity>().Where(predicate).Select(expression).FirstAsync();
        }
        #endregion

        #region 查询列表，TEntity 查询扩展
        public async Task<List<TEntity>> QueryList(string strOrderByFileds)
        {
            return await Context.Queryable<TEntity>().OrderBy(strOrderByFileds).ToListAsync();
        }

        public async Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> predicate, string strOrderByFileds)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate).OrderByIF(strOrderByFileds != null, strOrderByFileds).ToListAsync();
        }

        public async Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate)
                .OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).ToListAsync();
        }
        #endregion

        #region 查询列表, select<dto>() 用法
        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, bool>> predicate = null)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate).Select<TResult>().ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(string strOrderByFileds)
        {
            return await Context.Queryable<TEntity>().OrderBy(strOrderByFileds).Select<TResult>().ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await Context.Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).Select<TResult>().ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, bool>> predicate, string strOrderByFileds)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate).OrderByIF(strOrderByFileds != null, strOrderByFileds).Select<TResult>().ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate)
                .OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).Select<TResult>().ToListAsync();
        }
        #endregion

        #region 查询列表, select(expression) 表达式用法
        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate = null)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate).Select(expression).ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, string strOrderByFileds)
        {
            return await Context.Queryable<TEntity>().OrderBy(strOrderByFileds).Select(expression).ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await Context.Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).Select(expression).ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate, string strOrderByFileds)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate).OrderByIF(strOrderByFileds != null, strOrderByFileds).Select(expression).ToListAsync();
        }

        public async Task<List<TResult>> QueryList<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await Context.Queryable<TEntity>().WhereIF(predicate != null, predicate)
                .OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).Select(expression).ToListAsync();
        }
        #endregion

        #region 查询列表, 分页用法
        public async Task<List<TEntity>> QueryList(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null)
        {
            return await Context.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(whereExpression != null, whereExpression)
                .ToPageListAsync(pageParam.PageIndex, pageParam.PageSize);
        }

        public async Task<PagedInfo<TEntity>> QueryPageList(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await Context.Queryable<TEntity>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(pageParam.PageIndex, pageParam.PageSize, totalCount);

            return new PagedInfo<TEntity>
            {
                PageIndex = pageParam.PageIndex,
                PageSize = pageParam.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageParam.PageSize),
                DataSource = list
            };
        }

        public async Task<PagedInfo<TEntity>> QueryPageList(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            RefAsync<int> totalCount = 0;
            var list = await Context.Queryable<TEntity>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc)
                .ToPageListAsync(pageParam.PageIndex, pageParam.PageSize, totalCount);

            return new PagedInfo<TEntity>
            {
                PageIndex = pageParam.PageIndex,
                PageSize = pageParam.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageParam.PageSize),
                DataSource = list
            };
        }

        public async Task<PagedInfo<TResult>> QueryPageList<TResult>(Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await Context.Queryable<TEntity>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select<TResult>()
                .ToPageListAsync(pageParam.PageIndex, pageParam.PageSize, totalCount);

            return new PagedInfo<TResult>
            {
                PageIndex = pageParam.PageIndex,
                PageSize = pageParam.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageParam.PageSize),
                DataSource = list
            };
        }

        public async Task<PagedInfo<TResult>> QueryPageList<TResult>(Expression<Func<TEntity, TResult>> selectExpression, Expression<Func<TEntity, bool>> whereExpression, PageParm pageParam, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await Context.Queryable<TEntity>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToPageListAsync(pageParam.PageIndex, pageParam.PageSize, totalCount);

            return new PagedInfo<TResult>
            {
                PageIndex = pageParam.PageIndex,
                PageSize = pageParam.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageParam.PageSize),
                DataSource = list
            };
        }
        #endregion

        #region 事务委托
        /// <summary>
        /// 多租户事务
        /// </summary>
        public async Task<DbResult<bool>> UseITenantTran(Func<Task> action)
        {
            var resultTran = await itenant.UseTranAsync(async () =>
            {
                await action();
            });
            return resultTran;
        }

        /// <summary>
        /// 同连接事务
        /// </summary>
        public async Task<DbResult<bool>> UseTran(Func<Task> action)
        {
            var resultTran = await Context.Ado.UseTranAsync(async () =>
            {
                await action();
            });
            return resultTran;
        }
        #endregion
    }
}
