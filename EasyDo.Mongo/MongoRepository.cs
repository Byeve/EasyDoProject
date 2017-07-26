﻿using MongoDB.Driver;
using EasyDo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyDo.Mongo
{
    public  class MongoRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        private IMongoCollection<TEntity> primaryMongoCollection;

        private IMongoCollection<TEntity> secondaryMongoCollection;
        public MongoRepository(MongoDbContext dbContext)
        {
            //主库
            primaryMongoCollection = dbContext.PrimaryMongoCollection<TEntity>();

            //从库
            secondaryMongoCollection = dbContext.SecondaryMongoCollection<TEntity>();
        }
        private  bool IsSoftEntity()
        {
            return typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity));
        }
        #region Delete

        public virtual  bool Delete(TEntity entity)
        {
            return Delete(entity.Id);
        }

        public virtual Task<bool> DeleteAsync(TEntity entity)
        {
            return Task.Run(() =>
            {
                return Delete(entity);
            });
        }
        public virtual bool Delete(TPrimaryKey id)
        {
            if (IsSoftEntity())
            {
                return primaryMongoCollection.UpdateOne(m => m.Id.Equals(id), Builders<TEntity>.Update.Set(i => ((ISoftDelete)i).IsDeleted, true)).IsAcknowledged;
            }
            return primaryMongoCollection.DeleteOne(i => i.Id.Equals(id)).IsAcknowledged;
        }

        public virtual Task<bool> DeleteAsync(TPrimaryKey id)
        {
            return Task.Run(() =>
            {
                return Delete(id);
            });
        }

        public virtual bool Delete(Expression<Func<TEntity, bool>> filter)
        {
            if (IsSoftEntity())
            {
                return primaryMongoCollection.UpdateMany(filter, Builders<TEntity>.Update.Set(i => ((ISoftDelete)i).IsDeleted, true)).IsAcknowledged;
            }
            return primaryMongoCollection.DeleteMany(filter).IsAcknowledged;
        }

        public virtual Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            return Task.Run(() =>
            {
                return Delete(filter);
            });
        }

        public virtual bool DeleteAll()
        {
            if (IsSoftEntity())
            {
                return primaryMongoCollection.UpdateMany(Builders<TEntity>.Filter.Empty, Builders<TEntity>.Update.Set(i => ((ISoftDelete)i).IsDeleted, true)).IsAcknowledged;
            }
            return primaryMongoCollection.DeleteMany(Builders<TEntity>.Filter.Empty).IsAcknowledged;
        }

        public virtual Task<bool> DeleteAllAsync()
        {
            return Task.Run(() =>
            {
                return DeleteAll();
            });
        }
        #endregion Delete

        #region Select

        public virtual IQueryable<TEntity> Table
        {
            get
            {
                if (IsSoftEntity())
                {
                    return secondaryMongoCollection.AsQueryable().Where(i => ((ISoftDelete)i).IsDeleted == false);
                }
                return secondaryMongoCollection.AsQueryable();
            }
        }
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter)
        {
            return Table.Where(filter).ToList();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, int pageIndex, int size)
        {
            return Find(filter, i => i.Id, pageIndex, size);
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size)
        {
            return Find(filter, order, pageIndex, size, true);
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending)
        {
            var query = isDescending ? Table.Where(filter).OrderByDescending(order) : Table.Where(filter).OrderBy(order);

            return query.Skip(pageIndex * size).Take(size).ToList();
        }
        public virtual IEnumerable<TEntity> FindAll()
        {
            return Table.ToList();

        }
        public virtual IEnumerable<TEntity> FindAll(int pageIndex, int size)
        {
            return FindAll(i => i.Id, pageIndex, size);
        }
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size)
        {
            return FindAll(order, pageIndex, size, true);
        }
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, object>> order, int pageIndex, int size, bool isDescending)
        {
            var query = isDescending ? Table.OrderByDescending(order) : Table.OrderBy(order);
            return query.Skip(pageIndex * size).Take(size).ToList();
        }

        public virtual TEntity First()
        {
            return FindAll(i => i.Id, 0, 1, false).FirstOrDefault();
        }

        public virtual TEntity First(Expression<Func<TEntity, bool>> filter)
        {
            return First(filter, i => i.Id);
        }
        public virtual TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order)
        {
            return First(filter, order, false);
        }

        public virtual TEntity First(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending)
        {
            return Find(filter, order, 0, 1, isDescending).FirstOrDefault();
        }
        public virtual TEntity Get(TPrimaryKey id)
        {
            return Find(i => i.Id.Equals(id)).FirstOrDefault();
        }
        public virtual TEntity Last()
        {
            return FindAll(i => i.Id, 0, 1, true).FirstOrDefault();
        }

        public virtual TEntity Last(Expression<Func<TEntity, bool>> filter)
        {
            return Last(filter, i => i.Id);
        }
        public virtual TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order)
        {
            return Last(filter, order, false);
        }

        public virtual TEntity Last(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> order, bool isDescending)
        {
            return First(filter, order, !isDescending);
        }
        #endregion

        #region Insert
        public virtual void Insert(TEntity entity)
        {
            primaryMongoCollection.InsertOne(entity);
        }

        public virtual Task InsertAsync(TEntity entity)
        {
            return primaryMongoCollection.InsertOneAsync(entity);
        }

        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            primaryMongoCollection.InsertMany(entities);
        }

        public virtual Task InsertAsync(IEnumerable<TEntity> entities)
        {
            return primaryMongoCollection.InsertManyAsync(entities);
        }
        #endregion

        #region Update

        public virtual bool Update(TEntity entity)
        {
            return primaryMongoCollection.ReplaceOne(i => i.Id.Equals(entity.Id), entity).IsAcknowledged;
        }

        public virtual Task<bool> UpdateAsync(TEntity entity)
        {
            return Task.Run(() =>
            {
                return Update(entity);
            });
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                Update(entity);
            }
        }
        #endregion

        #region Aggregates

        public virtual bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return Table.Where(filter).Any();
        }

        public virtual long Count(Expression<Func<TEntity, bool>> filter)
        {
            return Table.Where(filter).LongCount();
        }

        public virtual Task<long> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            return Task.Run(() => {
                return Count(filter);
            });
        }

        public virtual long Count()
        {
            return Table.LongCount();
        }

        public virtual Task<long> CountAsync()
        {
            return Task.Run(() => {
                return Count();
            });
        }

        #endregion Utils
    }
    //public class MongoRepositoryOfStringPrimaryKey<TEntity> : MongoRepository<TEntity, string>, IRepository<TEntity> where TEntity : class, IEntity<string>
    //{
    //    public MongoRepositoryOfStringPrimaryKey(MongoDbContext dbContext) : base(dbContext)
    //    {
    //    }
    //}
}
