using EasyDo.Dependency;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EasyDo.Application
{
    public interface IApplicationService<TEntity> : IApplicationService
    {
        bool Delete(TEntity entity);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending =true);
        bool Any(Expression<Func<TEntity, bool>> filter);
        long Count(Expression<Func<TEntity, bool>> filter);
        TEntity GetById(string Id);
        void Insert(TEntity entity);
        void Insert(IEnumerable<TEntity> entities);
        bool Update(TEntity entity);
    }

    public interface IApplicationService: ITransientDependency
    {

    }
}
