using EasyDo.Configuration;
using EasyDo.Domain;
using MongoDB.Driver;
using System.Collections.Concurrent;
namespace EasyDo.Mongo
{
    public class MongoDbContext : IMongoDbContext
    {
        private static ConcurrentDictionary<string, MongoClient> MongoClients = new ConcurrentDictionary<string, MongoClient>();

        private readonly EasyDoConfiguration EasyDoConfiguration;
        private readonly EntityManager entityManager;

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
        private IMongoDatabase MasterDatabase(string DbName)
        {

            var databaseConnectionString = EasyDoConfiguration.MasterDataBaseConnectionString(DbName);
            if (MongoClients.ContainsKey(databaseConnectionString))
            {
                return MongoClients[databaseConnectionString].GetDatabase(DbName);
            }

            var mongoClient = new MongoClient(databaseConnectionString);
            if (!MongoClients.ContainsKey(databaseConnectionString))
            {
                MongoClients.TryAdd(databaseConnectionString, mongoClient);
            }
            return mongoClient.GetDatabase(DbName);
        }

        /// <summary>
        /// 获取从库
        /// </summary>
        /// <param name="DbName"></param>
        /// <returns></returns>
        private IMongoDatabase SlaveDatabase(string DbName)
        {

            var databaseConnectionString = EasyDoConfiguration.SlaveDataBaseConnectionString(DbName);
            if (MongoClients.ContainsKey(databaseConnectionString))
            {
                return MongoClients[databaseConnectionString].GetDatabase(DbName);
            }
            var mongoClient = new MongoClient(databaseConnectionString);
            MongoClients.TryAdd(databaseConnectionString, mongoClient);
            return mongoClient.GetDatabase(DbName);
        }

        /// <summary>
        /// 获取主库 MongoCollection
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>IMongoCollection</returns>
        public IMongoCollection<TEntity> MasterMongoCollection<TEntity>() where TEntity : class
        {
            var entityDescribe = GetEntityDescribe<TEntity>();

            return MasterDatabase(entityDescribe.DbName).GetCollection<TEntity>(entityDescribe.TableName);
            
        }

        /// <summary>
        /// 获取从库MongoCollection
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>IMongoCollection</returns>
        public IMongoCollection<TEntity> SlaveMongoCollection<TEntity>() where TEntity : class
        {
            //获取实体对象信息
            var entityDescribe = GetEntityDescribe<TEntity>();

            if (entityDescribe.ReadSecondary && EasyDoConfiguration.EnableSecondaryDB(entityDescribe.DbName))
            {
                return SlaveDatabase(entityDescribe.DbName).GetCollection<TEntity>(entityDescribe.TableName);
            }

            return MasterDatabase(entityDescribe.DbName).GetCollection<TEntity>(entityDescribe.TableName);
        }

        /// <summary>
        /// 是否可以软删除
        /// </summary>
        /// <returns></returns>
        public bool EnableSoftDelete<TEntity>()
        {
            var entityDescribe = GetEntityDescribe<TEntity>();

            return EasyDoConfiguration.EnableSoftDelete(entityDescribe.DbName);
        }

        private EntityDescribe GetEntityDescribe<TEntity>()
        {
            //获取实体对象信息
            return entityManager.GetEntityDescribe(typeof(TEntity));
        }
    }
}
