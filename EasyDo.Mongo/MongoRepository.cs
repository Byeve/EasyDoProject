using MongoDB.Driver;
using EasyDo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyDo.Mongo
{
    public  class MongoRepository<TEntity, TPrimaryKey> : IRepositoryRoot<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly IMongoDbContext _dbContext;

        //初始化
        public MongoRepository(IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //主库
        private IMongoCollection<TEntity> PrimaryMongoCollection => _dbContext.MasterMongoCollection<TEntity>();

        //从库
        private IMongoCollection<TEntity> SecondaryMongoCollection => _dbContext.SlaveMongoCollection<TEntity>();

        //是否软删除
        private  bool IsSoftEntity()
        {
            return _dbContext.EnableSoftDelete<TEntity>() && typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity));
        }
        #region Delete

        public virtual  bool Delete(TEntity entity)
        {
            return Delete(entity.Id);
        }

        public virtual Task<bool> DeleteAsync(TEntity entity)
        {
            return Task.Run(() => Delete(entity));
        }
        public virtual bool Delete(TPrimaryKey id)
        {
            return IsSoftEntity() ? PrimaryMongoCollection.UpdateOne(m => m.Id.Equals(id), Builders<TEntity>.Update.Set(i => ((ISoftDelete) i).IsDeleted, true)).IsAcknowledged : PrimaryMongoCollection.DeleteOne(i => i.Id.Equals(id)).IsAcknowledged;
        }

        public virtual Task<bool> DeleteAsync(TPrimaryKey id)
        {
            return Task.Run(() => Delete(id));
        }

        public virtual bool Delete(Expression<Func<TEntity, bool>> filter)
        {
            return IsSoftEntity() ? PrimaryMongoCollection.UpdateMany(filter, Builders<TEntity>.Update.Set(i => ((ISoftDelete)i).IsDeleted, true)).IsAcknowledged : PrimaryMongoCollection.DeleteMany(filter).IsAcknowledged;
        }

        public virtual Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            return Task.Run(() => Delete(filter));
        }

        public virtual bool DeleteAll()
        {
            return IsSoftEntity() ? PrimaryMongoCollection.UpdateMany(Builders<TEntity>.Filter.Empty, Builders<TEntity>.Update.Set(i => ((ISoftDelete)i).IsDeleted, true)).IsAcknowledged : PrimaryMongoCollection.DeleteMany(Builders<TEntity>.Filter.Empty).IsAcknowledged;
        }

        public virtual Task<bool> DeleteAllAsync()
        {
            return Task.Run(() => DeleteAll());
        }
        #endregion Delete

        #region Select

        public virtual IQueryable<TEntity> Table
        {
            get
            {
                return IsSoftEntity() ? SecondaryMongoCollection.AsQueryable().Where(i => ((ISoftDelete) i).IsDeleted == false) : SecondaryMongoCollection.AsQueryable();
            }
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
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter)
        {
            return Table.ToList();

        }

        public virtual TEntity First()
        {
            return Table.FirstOrDefault();
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            return Table.Where(filter).FirstOrDefault();
        }

        public virtual TEntity Get(TPrimaryKey id)
        {
            return Table.FirstOrDefault(m => m.Id.Equals(id));
        }
        #endregion

        #region Insert
        public virtual void Insert(TEntity entity)
        {
            PrimaryMongoCollection.InsertOne(entity);
        }

        public virtual Task InsertAsync(TEntity entity)
        {
            return PrimaryMongoCollection.InsertOneAsync(entity);
        }

        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            PrimaryMongoCollection.InsertMany(entities);
        }

        public virtual Task InsertAsync(IEnumerable<TEntity> entities)
        {
            return PrimaryMongoCollection.InsertManyAsync(entities);
        }
        #endregion

        #region Update

        public virtual bool Update(TEntity entity)
        {
            return PrimaryMongoCollection.ReplaceOne(i => i.Id.Equals(entity.Id), entity).IsAcknowledged;
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
