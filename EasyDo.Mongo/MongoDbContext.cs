using MongoDB.Driver;
using EasyDo.Configuration;
using EasyDo.Dependency;
using EasyDo.Domain;
using System;
using System.Collections.Generic;

namespace EasyDo.Mongo
{
    public class MongoDbContext: ISingletonDependency
    {
        private static Dictionary<string, MongoClient> MongoClients = new Dictionary<string, MongoClient>();

        private readonly EasyDoConfiguration EasyDoConfiguration;
        private readonly EntityManager entityManager;

        public MongoDbContext(EasyDoConfiguration EasyDoConfiguration, EntityManager entityManager)
        {
            this.EasyDoConfiguration = EasyDoConfiguration;
            this.entityManager = entityManager;
        }
        private IMongoDatabase GetDatabase(string DbName)
        {
            if (MongoClients.ContainsKey(DbName))
            {
                return MongoClients[DbName].GetDatabase(DbName);
            }
            else
            {
                var databaseConnectionString = EasyDoConfiguration.DatabaseConnectionString(DbName);
                if (string.IsNullOrEmpty(databaseConnectionString))
                {
                    throw new ArgumentException("数据库：{0} 连接未配置!");
                }
                var mongoClient = new MongoClient(databaseConnectionString);
                if (!MongoClients.ContainsKey(DbName))
                {
                    MongoClients.Add(DbName, mongoClient);
                }
                return mongoClient.GetDatabase(DbName);

            };
        }

        public IMongoCollection<TEntity> GetMongoCollection<TEntity>() where TEntity : class
        {
            var entityDescribe = entityManager.GetEntityDescribe(typeof(TEntity));

            return GetDatabase(entityDescribe.DbName).GetCollection<TEntity>(entityDescribe.TableName);
            
        }
    }
}
