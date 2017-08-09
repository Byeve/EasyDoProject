using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasyDo.Dependency;
using EasyDo.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EasyDo.Mongo
{
    public static class RepositoryRootExtend
    {
        public static IMongoCollection<TEntity> MongoCollection<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository) where TEntity : class, IEntity<TPrimaryKey>
        {
            var dbContext = IocManager.Instance.Resolve<MongoDbContext>();
            return dbContext.MasterMongoCollection<TEntity>();
        }

        /// <summary>
        /// 局部更新  此方法不支持嵌套类属性更新
        /// </summary>
        public static bool PartUpdate<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, TEntity entity, IEnumerable<string> fileds) where TEntity : class, IEntity<TPrimaryKey>
        {
            var doc = entity.ToBsonDocument();
            var updates = new List<UpdateDefinition<TEntity>>();
            foreach (var filed in fileds)
            {
                BsonElement bsonElement;
                doc.TryGetElement(filed, out bsonElement);
                updates.Add(Builders<TEntity>.Update.Set(filed, bsonElement.Value));
            }
            return PartUpdate(repository, entity.Id, updates.ToArray());
        }

        #region PartUpdate

        public static bool PartUpdate<TEntity, TPrimaryKey, TField>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, TEntity entity, Expression<Func<TEntity, TField>> field, TField value) where TEntity : class, IEntity<TPrimaryKey>
        {
            return PartUpdate(repository, entity, Builders<TEntity>.Update.Set(field, value));
        }

        public static Task<bool> UpdateAsync<TEntity, TPrimaryKey, TField>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, TEntity entity, Expression<Func<TEntity, TField>> field, TField value) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.Run(() => PartUpdate(repository, entity, Builders<TEntity>.Update.Set(field, value)));
        }


        public static bool PartUpdate<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, TPrimaryKey id, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            return PartUpdate(repository, Builders<TEntity>.Filter.Eq(i => i.Id, id), updates);
        }


        public static Task<bool> UpdateAsync<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, TPrimaryKey id, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.Run(() =>
            {
                return PartUpdate(repository, Builders<TEntity>.Filter.Eq(i => i.Id, id), updates);
            });
        }


        public static bool PartUpdate<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, TEntity entity, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            return PartUpdate(repository, entity.Id, updates);
        }


        public static Task<bool> UpdateAsync<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, TEntity entity, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.Run(() => PartUpdate(repository, entity.Id, updates));
        }

        public static bool PartUpdate<TEntity, TPrimaryKey, TField>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, FilterDefinition<TEntity> filter, Expression<Func<TEntity, TField>> field, TField value) where TEntity : class, IEntity<TPrimaryKey>
        {
            return PartUpdate(repository, filter, Builders<TEntity>.Update.Set(field, value));
        }


        public static Task<bool> UpdateAsync<TEntity, TPrimaryKey, TField>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, FilterDefinition<TEntity> filter, Expression<Func<TEntity, TField>> field, TField value) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.Run(() => PartUpdate(repository, filter, Builders<TEntity>.Update.Set(field, value)));
        }

        public static bool PartUpdate<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, FilterDefinition<TEntity> filter, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            var partUpdate = Builders<TEntity>.Update.Combine(updates);
            return MongoCollection(repository).UpdateMany(filter, partUpdate).IsAcknowledged;
        }

        public static Task<bool> UpdateAsync<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, FilterDefinition<TEntity> filter, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.Run(() => PartUpdate(repository, filter, updates));
        }

        public static bool PartUpdate<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> filter, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            var partUpdate = Builders<TEntity>.Update.Combine(updates);
            return MongoCollection(repository).UpdateMany(filter, partUpdate).IsAcknowledged;
        }

        public static Task<bool> UpdateAsync<TEntity, TPrimaryKey>(this IRepositoryRoot<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> filter, params UpdateDefinition<TEntity>[] updates) where TEntity : class, IEntity<TPrimaryKey>
        {
            return Task.Run(() => PartUpdate(repository, filter, updates));
        }

        #endregion PartUpdate
    }
}
