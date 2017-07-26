using EasyDo.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EasyDo.Application
{
    public abstract class CrudAppService<TEntity> : IApplicationService<TEntity> where TEntity : class, IEntity<string>
    {
        public readonly IRepository<TEntity> repository;
        public CrudAppService(IRepository<TEntity> repository)
        {
            this.repository = repository;
        }
        /// <summary>
        /// 根据主键获取
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>对象</returns>
        public virtual TEntity GetById(string Id)
        {
            return repository.Get(Id);
        }

        /// <summary>
        /// 条件分页
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="order">排序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="size">页码大小</param>
        /// <param name="isDescending">升序/降序</param>
        /// <returns>对象集合</returns>
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending =true)
        {
            return repository.Find(filter, order, pageIndex, size, isDescending);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return repository.Any(filter);
        }
        public virtual long Count(Expression<Func<TEntity, bool>> filter)
        {
            return repository.Count(filter);
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="entity">对象</param>
        public virtual void Insert(TEntity entity)
        {
            repository.Insert(entity);
        }
        /// <summary>
        /// 批量添加对象
        /// </summary>
        /// <param name="entities"></param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            repository.Insert(entities);
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns>操作结果</returns>
        public virtual bool Delete(TEntity entity)
        {
            return repository.Delete(entity);
        }
        /// <summary>
        /// 完整更新对象
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns>操作结果</returns>
        public virtual bool Update(TEntity entity)
        {
            return repository.Update(entity);
        }



    }
}
