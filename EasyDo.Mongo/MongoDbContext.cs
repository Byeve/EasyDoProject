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

        private const string PrimaryDB = "PrimaryDB";
        private const string SecondaryDB = "SecondaryDB";

        public MongoDbContext(EasyDoConfiguration EasyDoConfiguration, EntityManager entityManager)
        {
            this.EasyDoConfiguration = EasyDoConfiguration;
            this.entityManager = entityManager;
        }


        /// <summary>
        /// 获取主库
        /// </summary>
        /// <param name="DbName"></param>
        /// <returns></returns>
        private IMongoDatabase PrimaryDatabase(string DbName)
        {
            var dbKey = DbName + PrimaryDB;
            if (MongoClients.ContainsKey(dbKey))
            {
                return MongoClients[dbKey].GetDatabase(DbName);
            }
            var databaseConnectionString = EasyDoConfiguration.PrimaryDataBaseConnectionString(DbName);

            if (string.IsNullOrEmpty(databaseConnectionString))
            {
                throw new ArgumentException(string.Format("数据库：{0} 主库连接未配置!", DbName));
            }

            var mongoClient = new MongoClient(databaseConnectionString);
            if (!MongoClients.ContainsKey(dbKey))
            {
                MongoClients.Add(dbKey, mongoClient);
            }
            return mongoClient.GetDatabase(DbName);
        }

        /// <summary>
        /// 获取从库
        /// </summary>
        /// <param name="DbName"></param>
        /// <returns></returns>
        private IMongoDatabase SecondaryDatabase(string DbName)
        {
            var dbKey = DbName + SecondaryDB;
            if (MongoClients.ContainsKey(dbKey))
            {
                return MongoClients[dbKey].GetDatabase(DbName);
            }
            var databaseConnectionString = EasyDoConfiguration.SecondaryDataBaseConnectionString(DbName);

            if (string.IsNullOrEmpty(databaseConnectionString))
            {
                throw new ArgumentException(string.Format("数据库：{0} 从库连接未配置!", DbName));
            }

            var mongoClient = new MongoClient(databaseConnectionString);
            if (!MongoClients.ContainsKey(dbKey))
            {
                MongoClients.Add(dbKey, mongoClient);
            }
            return mongoClient.GetDatabase(DbName);
        }


        public IMongoCollection<TEntity> PrimaryMongoCollection<TEntity>() where TEntity : class
        {
            var entityDescribe = entityManager.GetEntityDescribe(typeof(TEntity));

            return PrimaryDatabase(entityDescribe.DbName).GetCollection<TEntity>(entityDescribe.TableName);
            
        }

        /// <summary>
        /// 获取从库MongoCollection
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns></returns>
        public IMongoCollection<TEntity> SecondaryMongoCollection<TEntity>() where TEntity : class
        {
            //获取实体对象信息
            var entityDescribe = entityManager.GetEntityDescribe(typeof(TEntity));

            if (entityDescribe.ReadSecondary && EasyDoConfiguration.EnableSecondaryDB)
            {
                return SecondaryDatabase(entityDescribe.DbName).GetCollection<TEntity>(entityDescribe.TableName);
            }

            return PrimaryDatabase(entityDescribe.DbName).GetCollection<TEntity>(entityDescribe.TableName);
        }
    }
}
