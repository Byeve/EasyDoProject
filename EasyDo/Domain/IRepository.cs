using EasyDo.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyDo.Domain
{
    public interface IRepository<TEntity, TPrimaryKey>  where TEntity : class, IEntity<TPrimaryKey>
    {
        IQueryable<TEntity> Table { get; }

        bool Any(Expression<Func<TEntity, bool>> filter);
        long Count();
        long Count(Expression<Func<TEntity, bool>> filter);
        Task<long> CountAsync();
        Task<long> CountAsync(Expression<Func<TEntity, bool>> filter);
        bool Delete(Expression<Func<TEntity, bool>> filter);
        bool Delete(TPrimaryKey id);
        bool Delete(TEntity entity);
        bool DeleteAll();
        Task<bool> DeleteAllAsync();
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter);
        Task<bool> DeleteAsync(TPrimaryKey id);
        Task<bool> DeleteAsync(TEntity entity);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending);
        IEnumerable<TEntity> FindAll();
        TEntity First();
        TEntity Get(TPrimaryKey id);
        void Insert(IEnumerable<TEntity> entities);
        void Insert(TEntity entity);
        Task InsertAsync(IEnumerable<TEntity> entities);
        Task InsertAsync(TEntity entity);
        void Update(IEnumerable<TEntity> entities);
        bool Update(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, string> where TEntity : class, IEntity<string>
    {

    }
}
